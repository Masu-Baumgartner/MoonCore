window.moonCore = {
    codeEditor: {
        instances: new Map(),
        attach: function (id, options) {
            const editor = ace.edit(id, options);

            moonCore.codeEditor.instances.set(id, editor);
        },
        updateOptions: function (id, options) {
            const editor = moonCore.codeEditor.instances.get(id);

            editor.setOptions(options);
        },
        getValue: function (id) {
            const editor = moonCore.codeEditor.instances.get(id);

            return editor.getValue();
        },
        destroy: function (id) {
            const editor = moonCore.codeEditor.instances.get(id);

            if (!editor)
                return;

            editor.destroy();
            editor.container.remove();

            moonCore.codeEditor.instances.delete(id);
        }
    },
    chunkedDownload: {
        instances: new Map(),

        start: function (downloadId, fileName) {
            let controller;

            const stream = new ReadableStream({
                start(ctrl) {
                    controller = ctrl;
                }
            });

            const response = new Response(stream);
            const blobPromise = response.blob();

            this.instances.set(downloadId, {
                controller,
                blobPromise,
                fileName
            });
        },

        writeChunk: function (downloadId, chunk) {
            const state = this.instances.get(downloadId);
            if (state && state.controller) {
                state.controller.enqueue(new Uint8Array(chunk));
            }
        },

        finish: async function (downloadId) {
            const state = this.instances.get(downloadId);
            if (state && state.controller) {
                state.controller.close();

                const blob = await state.blobPromise;
                const url = URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = state.fileName;
                a.click();
                URL.revokeObjectURL(url);

                this.instances.delete(downloadId);
            }
        }
    },
    dropzone: {
        items: [],
        isInitialized: false,
        isEnabled: false,

        init: function (dotNetHelper) {

            if (this.isInitialized)
                return;

            this.isInitialized = true;
            
            // Check which features are supported by the browser
            const supportsFileSystemAccessAPI =
                'getAsFileSystemHandle' in DataTransferItem.prototype;
            const supportsWebkitGetAsEntry =
                'webkitGetAsEntry' in DataTransferItem.prototype;

            // This is the drag and drop zone.
            const elem = document.body;

            elem.addEventListener("dragenter", async () => {

                if (!this.isEnabled)
                    return;

                await dotNetHelper.invokeMethodAsync("OnDragEnter");
            });

            elem.addEventListener("dragleave", async () => {

                if (!this.isEnabled)
                    return;

                await dotNetHelper.invokeMethodAsync("OnDragLeave");
            });

            // Prevent navigation.
            elem.addEventListener('dragover', (e) => {

                if (!this.isEnabled)
                    return;

                e.preventDefault();
            });

            elem.addEventListener('drop', async (e) => {

                if (!this.isEnabled)
                    return;

                // Prevent navigation.
                e.preventDefault();

                if (!supportsFileSystemAccessAPI && !supportsWebkitGetAsEntry) {
                    // Cannot handle directories.
                    console.log("Cannot handle directories");
                    return;
                }

                await this.getAllWebkitFileEntries(e.dataTransfer.items).then(async value => {
                    value.forEach(a => moonCore.dropzone.items.push(a));
                });

                await dotNetHelper.invokeMethodAsync("Trigger");
            });
        },
        enable: function () {
            this.isEnabled = true;
            this.items = [];
        },
        disable: function () {
            this.isEnabled = false;
        },

        peek: async function () {

            if (this.items.length === 0)
                return null;

            const item = this.items[this.items.length-1];

            if (!item)
            {
                console.log("No items to peek")
                return null;
            }

            let file;
            let path;

            if (item instanceof File) {
                file = item;
                path = file.name;
            } else {
                file = await this.openFileEntry(item);
                path = item.fullPath;
            }

            if (file.size === 0)
            {
                console.log("File is empty");
                
                return {
                    shouldSkipToNext: true
                };
            }

            const stream = await this.createStreamRef(file);

            if (!stream)
            {
                console.log("Stream failed");
                
                return {
                    shouldSkipToNext: true
                };
            }

            return {
                path: path,
                stream: stream,
                shouldSkipToNext: false
            }
        },

        pop: function () {
            this.items.pop();
        },

        openFileEntry: async function (fileEntry) {
            const promise = new Promise(resolve => {
                fileEntry.file(file => {
                    resolve(file);
                }, err => console.log(err));
            });

            return await promise;
        },
        createStreamRef: async function (processedFile) {

            // Prevent uploads of empty files
            if (processedFile.size <= 0) {
                console.log("Skipping upload of '" + processedFile.name + "' as its empty");
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

        getAllWebkitFileEntries: async function (dataTransferItemList) {
            function readAllEntries(reader) {
                return new Promise((resolve, reject) => {
                    const entries = [];

                    function readEntries() {
                        reader.readEntries((batch) => {
                            if (batch.length === 0) {
                                resolve(entries);
                            } else {
                                entries.push(...batch);
                                readEntries();
                            }
                        }, reject);
                    }

                    readEntries();
                });
            }

            async function traverseEntry(entry) {
                if (entry.isFile) {
                    return [entry];
                } else if (entry.isDirectory) {
                    const reader = entry.createReader();
                    const entries = await readAllEntries(reader);
                    const subEntries = await Promise.all(entries.map(traverseEntry));
                    return subEntries.flat();
                }
                return [];
            }

            const entries = [];

            // Convert DataTransferItemList to entries
            for (let i = 0; i < dataTransferItemList.length; i++) {
                const item = dataTransferItemList[i];
                const entry = item.webkitGetAsEntry();
                if (entry) {
                    entries.push(entry);
                }
            }

            // Traverse all entries and collect file entries
            const allFileEntries = await Promise.all(entries.map(traverseEntry));
            return allFileEntries.flat();
        }
    },
    downloadHelper: {
        downloadFileFromStream: async function (fileName, contentStreamReference) {
            const arrayBuffer = await contentStreamReference.arrayBuffer();
            const blob = new Blob([arrayBuffer]);
            const url = URL.createObjectURL(blob);

            const anchor = document.createElement('a');
            anchor.href = url;
            anchor.download = fileName || 'download';
            anchor.click();

            URL.revokeObjectURL(url);
        },
        downloadFileFromUrl: async function (url) {
            const anchor = document.createElement('a');
            anchor.href = url;
            anchor.click();
        }
    },
}