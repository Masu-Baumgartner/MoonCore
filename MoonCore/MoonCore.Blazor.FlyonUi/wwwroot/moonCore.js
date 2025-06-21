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
    }
}