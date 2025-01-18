window.moonCoreCodeEditor = {
    instances: new Map(),
    attach: function (id) {
        const editor = ace.edit(id);
        
        this.instances.set(id, editor);
    },
    destroy: function (id){
        const editor = this.instances.get(id);
        
        editor.destroy();
        editor.container.remove();
    }
}