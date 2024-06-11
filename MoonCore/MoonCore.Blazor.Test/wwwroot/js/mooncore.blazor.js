window.mooncore = {
    blazor: {
        modals: {
            show: function (id, focus)
            {
                let modal = bootstrap.Modal.getOrCreateInstance("#" + id, {
                    focus: focus
                });

                modal.show();
            },
            hide: function (id)
            {
                let modal = bootstrap.Modal.getOrCreateInstance("#" + id);
                modal.hide();
            }
        },
        toasts: {
            initAndShow: function(id) {
                var toast = bootstrap.Toast.getOrCreateInstance("#" + id, {
                    autohide: false
                });

                toast.show();
            }
        }
    }
}