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
        
        init: function (element, dotNetHelper) {
            // Check which features are supported by the browser
            const supportsFileSystemAccessAPI =
                'getAsFileSystemHandle' in DataTransferItem.prototype;
            const supportsWebkitGetAsEntry =
                'webkitGetAsEntry' in DataTransferItem.prototype;

            // This is the drag and drop zone.
            const elem = element;

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

                this.getAllWebkitFileEntries(e.dataTransfer.items).then(async value => {
                    value.forEach(a => moonCore.dropzone.items.push(a));
                });
            });
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
    }
}