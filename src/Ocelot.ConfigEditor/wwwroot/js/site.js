var configEditor = (function() {
    
    function getIndexValue(index) {
        const self = $("#" + index);
        return parseInt($(self).val());
    }
    
    return {
        setKeyValuePair: function(element, id, name) {
            element.next().attr("id", `${id}_${$(element).val()}_`)
                .attr("name", `${name}[${$(element).val()}]`);
        },
        appendRequestTextBox: function(element, className) {
            element
                .append(
                    `<div class="form-group"> 
                    <input type="text" placeholder="Enter header key" class="${className}"/> 
                    <input type="text" placeholder="Enter header value"/> 
                    <a href="#" class="btn btn-xs btn-default textbox-remove">
                        <span class="fas fa-remove"></span>
                    </a>
                    </div>`);
        },
        appendScopeTextBox: function(element) {
            const index = "allowed-scope-index";
            const indexValue = getIndexValue(index);
            element
                .append(
                    `<div class="form-group"> 
                    <input type="text" id="FileReRoute_AuthenticationOptions_AllowedScopes_${indexValue}_" name="FileReRoute.AuthenticationOptions.AllowedScopes[${indexValue}]" placeholder="Enter scope"/> 
                    <a href="#" class="btn btn-xs btn-default textbox-remove" data-index="${index}">
                        <span class="fas fa-remove"></span>
                    </a>
                    </div>`);
            
            this.incrementIndex(index);
        },
        appendHostPortTextBox: function(element) {
            const index = "host-port-index";
            const indexValue = getIndexValue(index);
            element
                .append(
                    `<div class="form-group">
                    <input type="text" placeholder="Enter host" id="FileReRoute_DownstreamHostAndPorts_${indexValue}_Host" name="FileReRoute.DownstreamHostAndPorts[${indexValue}].Host"/> 
                    <input type="text" placeholder="Enter port" id="FileReRoute_DownstreamHostAndPorts_${indexValue}_Port" name="FileReRoute.DownstreamHostAndPorts[${indexValue}].Port"/> 
                    <a href="#" class="btn btn-xs btn-default textbox-remove" data-index="${index}">
                        <span class="fas fa-remove"></span>
                    </a>
                    </div>`);
            
            this.incrementIndex(index);
        },
        incrementIndex: function(index) {
            const self = $("#" + index);
            $(self).val(getIndexValue(index) + 1);
        },
        decrementIndex: function(index) {
            const self = $("#" + index);
            $(self).val(getIndexValue(index) - 1);
        },
        getNotifyDefaults: function() {
            return $.notifyDefaults({ template: "<div data-notify=\"container\" class=\"col-xs-11 col-sm-3 alert alert-{0}\" role=\"alert\">" +
                    "<button type=\"button\" aria-hidden=\"true\" class=\"close\" data-notify=\"dismiss\">×</button>" +
                    "<span data-notify=\"icon\"></span> " +
                    "<span data-notify=\"title\">{1}</span> " +
                    "<span data-notify=\"message\">{2}</span>" +
                    "<div class=\"progress\" data-notify=\"progressbar\">" +
                    "<div class=\"progress-bar progress-bar-{0}\" role=\"progressbar\" aria-valuenow=\"0\" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width: 0%;\"></div>" +
                    "</div>" +
                    "<a href=\"{3}\" target=\"{4}\" data-notify=\"url\"></a>" +
                    "</div>" });
        }
    };
}());

$(document).ready(function() {
    $("#sign-out").on("click", 
        function(e) {
            $("#sign-out-form").submit();
        });
    
    $("#button-save").on("click",
        function(e) {
            $("#form-save").submit();
        });

    $("#button-delete").on("click",
        function (e) {
            const result = confirm("Are you sure you want to delete this reroute?");
            if (result) {
                $("#form-delete").submit();
            }
        });

    $("#button-cancel").on("click",
        function(e) {
            window.location.href = $(this).attr("data-action");
        });

    $(document).on("input propertychange paste",
        ".headers-key",
        function() {
            configEditor.setKeyValuePair($(this), "FileReRoute_AddHeadersToRequest", "FileReRoute.AddHeadersToRequest");
        });

    $(document).on("input propertychange paste",
        ".claims-key",
        function() {
            configEditor.setKeyValuePair($(this), "FileReRoute_AddClaimsToRequest", "FileReRoute.AddClaimsToRequest");
        });

    $(document).on("input propertychange paste",
        ".queries-key",
        function() {
            configEditor.setKeyValuePair($(this), "FileReRoute_AddQueriesToRequest", "FileReRoute.AddQueriesToRequest");
        });

    $(document).on("click",
        ".textbox-remove",
        function(e){
            $(this).parent().remove();
            if ($(this).data("index").length > 0)
                configEditor.decrementIndex($(this).data("index"));
            
            e.preventDefault();
        });
    
    $("#button-headers-add").on("click",
        function(e) {
            configEditor.appendRequestTextBox($("#request-headers"), "headers-key");
            e.preventDefault();
        });

    $("#button-claims-add").on("click",
        function(e) {
            configEditor.appendRequestTextBox($("#request-claims"), "claims-key");
            e.preventDefault();
        });

    $("#button-queries-add").on("click",
        function(e) {
            configEditor.appendRequestTextBox($("#request-queriess"), "queries-key");
            e.preventDefault();
        });

    $("#button-allowedscope-add").on("click",
        function(e) {
            configEditor.appendScopeTextBox($("#allowed-scope"));
            e.preventDefault();
        });
    
    $("#button-hostport-add").on("click",
        function(e) {
            configEditor.appendHostPortTextBox($("#host-port"));
            e.preventDefault();
        });

    $('[data-toggle="tooltip"]').tooltip();
});