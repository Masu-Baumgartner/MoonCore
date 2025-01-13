window.moonCoreFileManager = {
    uploadCache: [],
    readCache: async function (index) {
        const item = this.uploadCache.at(index);
        const streamRef = await this.createStreamRef(item);
        
        return {
            path: item.fullPath,
            streamRef: streamRef
        };
    },
    createStreamRef: async function (fileEntry)
    {
        const promise = new Promise(resolve => {
            fileEntry.file(file => {
                resolve(file);
            }, err => console.log(err));
        });

        const processedFile = await promise;

        // Prevent uploads of empty files
        if (processedFile.size <= 0) {
            console.log("Skipping upload of '" + fileEntry.fullPath + "' as its empty");
            return null;
        }

        const fileReader = new FileReader();

        const readerPromise = new Promise(resolve => {
            fileReader.addEventListener("loadend", ev => {
                resolve(fileReader.result)
            });
        });

        fileReader.readAsArrayBuffer(processedFile);

        const arrayBuffer = await readerPromise;

        return DotNet.createJSStreamReference(arrayBuffer);
    },
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

            // Store are loaded entries here
            const contentEntries = [];

            for (let i = 0; i < e.dataTransfer.items.length; i++) {
                const fileItem = e.dataTransfer.items[i];

                const webkitEntry = fileItem.webkitGetAsEntry();

                if (webkitEntry.isFile)
                    contentEntries.push(webkitEntry);
                else if (webkitEntry.isDirectory) {
                    const entries = await this.traverseDirectory(webkitEntry);

                    for (const directoryEntry of entries) {
                        contentEntries.push(directoryEntry)
                    }
                }
            }

            this.uploadCache = contentEntries;
            await callbackRef.invokeMethodAsync("OnFilesDropped", this.uploadCache.length);
        });
    },
    traverseDirectory: async function (directoryEntry) {
        let files = [];

        // Function to handle reading the directory entries
        async function readDirectory(entry) {
            const reader = entry.createReader();

            // Function to read entries in batches
            const readEntriesBatch = () => {
                return new Promise((resolve, reject) => {
                    reader.readEntries((entries) => {
                        if (entries.length === 0) {
                            resolve([]);
                        } else {
                            resolve(entries);
                        }
                    }, reject);
                });
            };

            let entries;
            do {
                entries = await readEntriesBatch();
                for (const entry of entries) {
                    if (entry.isFile) {
                        files.push(entry); // Add file entry to the list
                    } else if (entry.isDirectory) {
                        // Recursively traverse sub-directories
                        await readDirectory(entry);
                    }
                }
            } while (entries.length > 0);  // Repeat until no more entries are left
        }

        // Start reading from the given directory
        await readDirectory(directoryEntry);

        return files;
    }
}