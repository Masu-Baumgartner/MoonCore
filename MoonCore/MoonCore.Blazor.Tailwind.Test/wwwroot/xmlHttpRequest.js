window.moonCoreXmlHttpRequest = {
    storage: {},
    initialize: function (trackingId, refObject) {
        const req = new XMLHttpRequest();
        
        req.addEventListener("progress", async ev => {
            await refObject.invokeMethodAsync("TriggerProgressEvent", ev); 
        });

        req.addEventListener("loaded", async ev => {
            await refObject.invokeMethodAsync("TriggerLoadedEvent", ev);
        });
        
        this.storage[trackingId] = req;
        
        return req;
    },
    setProperty: function (trackingId, property, value) {
        this.storage[trackingId][property] = value;
    },
    getProperty: function (trackingId, property) {
        return this.storage[trackingId][property];
    },
    getResponseStream: function (trackingId) {
        const stream = this.storage[trackingId].response.stream();
        return DotNet.createJSStreamReference(stream);
    },
    dispose: function (trackingId) {
        this.storage[trackingId] = undefined;
    }
}