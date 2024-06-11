window.mooncore = {
    blazor: {
        modals: {
            show: function (id, focus) {
                let modal = bootstrap.Modal.getOrCreateInstance("#" + id, {
                    focus: focus
                });

                modal.show();
            },
            hide: function (id) {
                let modal = bootstrap.Modal.getOrCreateInstance("#" + id);
                modal.hide();
            }
        },
        toasts: {
            initAndShow: function (id) {
                var toast = bootstrap.Toast.getOrCreateInstance("#" + id, {
                    autohide: false
                });

                toast.show();
            }
        },
        misc: {
            download: async function (fileName, contentStreamReference) {
                const arrayBuffer = await contentStreamReference.arrayBuffer();
                const blob = new Blob([arrayBuffer]);
                const url = URL.createObjectURL(blob);
                const anchorElement = document.createElement('a');

                anchorElement.href = url;
                anchorElement.download = fileName ?? '';

                anchorElement.click();

                anchorElement.remove();
                URL.revokeObjectURL(url);
            }
        },
        clipboard: {
            copy: function (text) {
                if (!navigator.clipboard) {
                    const textArea = document.createElement("textarea");
                    textArea.value = text;

                    // Avoid scrolling to bottom
                    textArea.style.top = "0";
                    textArea.style.left = "0";
                    textArea.style.position = "fixed";

                    document.body.appendChild(textArea);
                    textArea.focus();
                    textArea.select();

                    try {
                        const successful = document.execCommand('copy');
                        const msg = successful ? 'successful' : 'unsuccessful';
                    } catch (err) {
                        console.error('Fallback: Oops, unable to copy', err);
                    }

                    document.body.removeChild(textArea);
                    return;
                }
                navigator.clipboard.writeText(text).then(function () {
                    },
                    function (err) {
                        console.error('Async: Could not copy text: ', err);
                    }
                );
            },
            readTypes: async function() {
                const clipboardContents = await navigator.clipboard.read();
                const content = clipboardContents[0];

                return content.types;
            },
            readData: async function(maxSize, contentType) {
                const clipboardContents = await navigator.clipboard.read();
                const content = clipboardContents[0];
                
                function blobToBase64(blob) {
                    return new Promise((resolve, _) => {
                        const reader = new FileReader();
                        reader.onloadend = () => resolve(reader.result);
                        reader.readAsDataURL(blob);
                    });
                }

                return await blobToBase64(await content.getType(contentType));
            }
        }
    }
}