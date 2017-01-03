function Ajax() {

    /*
        Maybe this is really stupid. Previously I was calling C# methods and getting the
        response directly from AjaxObject, rather than having C# make an additional JS 
        call. The problem was that JS seems unable to make multiple consecutive "ajax" calls;
        once the first had been fired it sat and waited for the response - even when chaining 
        the promise returned from CEF with .then(). Maybe there's a better way to make several
        C# calls simultaneously, but I couldn't find much when looking online.
     */

    var self = this;

    self.pending = {};

    self.execute = function (controllerName, methodName, data, callback) {
        var guid = self.guid();
        self.pending[guid] = callback;
        csharpAjax.execute(guid, controllerName, methodName, data);
    }

    self.resolve = function (guid, data) {
        var callback = self.pending[guid];
        if (callback) {
            // if I call JSON.parse here it fails, but if I do exactly the same immediately inside the callback it succeeds
            // maybe because this method is being called from C# using ExecuteScriptAsync?
            callback(data);
        }

        delete self.pending[guid];
    }

    // TODO does this reliably generate unique IDs?
    self.guid = function () {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
              .toString(16)
              .substring(1);
        }
        return s4() + s4() + "-" + s4() + "-" + s4() + "-" +
          s4() + "-" + s4() + s4() + s4();
    }
}

// global
var ajax = new Ajax();