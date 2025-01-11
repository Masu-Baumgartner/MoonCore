window.moonCoreFileManager = {
    setup: function (id, callbackRef) {
        // Check which features are supported by the browser
        const supportsFileSystemAccessAPI =
            'getAsFileSystemHandle' in DataTransferItem.prototype;
        const supportsWebkitGetAsEntry =
            'webkitGetAsEntry' in DataTransferItem.prototype;

        // This is the drag and drop zone.
        const elem = document.getElementById(id);

        // Prevent navigation.
        elem.addEventListener('dragover', (e) => {
            e.preventDefault();
        });

        elem.addEventListener('drop', async (e) => {

            // Prevent navigation.
            e.preventDefault();

            if (!supportsFileSystemAccessAPI && !supportsWebkitGetAsEntry) {
                // Cannot handle directories.
                console.log("Cannot handle directories");
                return;
            }

            const files = [];
            
            async function getDirectoryContents(entry) {
                const result = [];

                let reader = entry.createReader();
                
                // Resolved when the entire directory is traversed
                await new Promise((resolve_directory) => {
                    var iteration_attempts = [];
                    (function read_entries() {
                        // According to the FileSystem API spec, readEntries() must be called until
                        // it calls the callback with an empty array.  Seriously??
                        reader.readEntries((entries) => {
                            if (!entries.length) {
                                // Done iterating this particular directory
                                resolve_directory(Promise.all(iteration_attempts));
                            } else {
                                // Add a list of promises for each directory entry.  If the entry is itself 
                                // a directory, then that promise won't resolve until it is fully traversed.
                                iteration_attempts.push(Promise.all(entries.map(async (entry) => {
                                    if (entry.isFile) {
                                        console.log();
                                        
                                        result.push(entry);
                                        return entry;
                                    } else {
                                        // DO SOMETHING WITH DIRECTORIES
                                        return await getDirectoryContents(entry);
                                    }
                                })));
                                // Try calling readEntries() again for the same dir, according to spec
                                read_entries();
                            }
                        }, err => console.log(err));
                    })();
                });
                
                return result;
            }

            for (let i = 0; i < e.dataTransfer.items.length; i++) {
                const fileItem = e.dataTransfer.items[i];

                const webkitEntry = fileItem.webkitGetAsEntry();

                if (webkitEntry.isFile)
                    files.push(webkitEntry);
                else if (webkitEntry.isDirectory) {
                    const entries = await getDirectoryContents(webkitEntry);

                    for (const directoryEntry of entries) {
                        files.push(directoryEntry)
                    }
                }
            }

            await callbackRef.invokeMethodAsync("UpdateState", 0, files.length, true);

            for (let i = 0; i < files.length; i++) {
                const promise = new Promise(resolve => {
                    files[i].file(file => {
                        resolve(file);
                    }, err => console.log(err));
                });

                const processedFile = await promise;

                const fileReader = new FileReader();

                const readerPromise = new Promise(resolve => {
                    fileReader.addEventListener("loadend", ev => {
                        resolve(fileReader.result)
                    });
                });

                fileReader.readAsArrayBuffer(processedFile);

                const arrayBuffer = await readerPromise;
                const interopStream = DotNet.createJSStreamReference(arrayBuffer);

                await callbackRef.invokeMethodAsync("UpdateState", i + 1, files.length, true);
                await callbackRef.invokeMethodAsync("PushFile", files[i].fullPath, interopStream);
            }

            await callbackRef.invokeMethodAsync("UpdateState", files.length, files.length, false);
        });
    }
}