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
                        document.execCommand('copy');
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
            readTypes: async function () {
                const clipboardContents = await navigator.clipboard.read();
                const content = clipboardContents[0];

                return content.types;
            },
            readData: async function (contentType, maxSize) {
                const clipboardContents = await navigator.clipboard.read();
                const content = clipboardContents[0];

                function blobToBase64(blob) {
                    return new Promise((resolve, _) => {
                        const reader = new FileReader();
                        reader.onloadend = () => resolve(reader.result);
                        reader.readAsDataURL(blob);
                    });
                }

                const blob = await content.getType(contentType);

                if (blob.size > maxSize)
                    return "ERRSIZE";

                return await blobToBase64(blob);
            }
        },
        debugConsole: {
            reference: {},

            register: function (ref) {
                this.reference = ref;

                document.addEventListener("keydown", async (event) => {
                    if (event.code === "Enter" && event.ctrlKey)
                        await this.reference.invokeMethodAsync("Toggle");
                });
            },

            initWindow: function (id) {
                var element = document.getElementById(id);
                
                // Make an element draggable (or if it has a .window-top class, drag based on the .window-top element)
                let currentPosX = 0, currentPosY = 0, previousPosX = 0, previousPosY = 0;

                // If there is a window-top classed element, attach to that element instead of full window
                if (element.querySelector('.window-top')) {
                    // If present, the window-top element is where you move the parent element from
                    element.querySelector('.window-top').onmousedown = dragMouseDown;
                } else {
                    // Otherwise, move the element itself
                    element.onmousedown = dragMouseDown;
                }

                function dragMouseDown(e) {
                    // Prevent any default action on this element (you can remove if you need this element to perform its default action)
                    e.preventDefault();
                    // Get the mouse cursor position and set the initial previous positions to begin
                    previousPosX = e.clientX;
                    previousPosY = e.clientY;
                    // When the mouse is let go, call the closing event
                    document.onmouseup = closeDragElement;
                    // call a function whenever the cursor moves
                    document.onmousemove = elementDrag;
                }

                function elementDrag(e) {
                    // Prevent any default action on this element (you can remove if you need this element to perform its default action)
                    e.preventDefault();
                    // Calculate the new cursor position by using the previous x and y positions of the mouse
                    currentPosX = previousPosX - e.clientX;
                    currentPosY = previousPosY - e.clientY;
                    // Replace the previous positions with the new x and y positions of the mouse
                    previousPosX = e.clientX;
                    previousPosY = e.clientY;
                    // Set the element's new position
                    element.style.top = (element.offsetTop - currentPosY) + 'px';
                    element.style.left = (element.offsetLeft - currentPosX) + 'px';
                }

                function closeDragElement() {
                    // Stop moving when mouse button is released and release events
                    document.onmouseup = null;
                    document.onmousemove = null;
                }
            }
        }
    }
}