var configEditor = (function() {
    function getAllowedScopeIndex() {
        return parseInt($("#allowed-scope-index").val());
    }

    function setAllowedScopeIndex(value) {
        $("#allowed-scope-index").val(value);
    }

    return {
        setKeyValuePair: function(element, id, name) {
            element.next().attr("id", `${id}_${$(element).val()}_`)
                .attr("name", `${name}[${$(element).val()}]`);
        },
        appendRequestTextBox: function(element, className) {
            element
                .append(
                    `<div class="form-group"> <input type="text" placeholder="Enter header key" class="${className}
                    "/> <input type="text" placeholder="Enter header value"/> <a href="#" class="btn btn-xs btn-default"><span class="glyphicon glyphicon-remove"></span></a></div>`);
        },
        appendScopeTextBox: function(element) {
            var index = getAllowedScopeIndex() + 1;
            setAllowedScopeIndex(index);
            element
                .append(
                `<div class="form-group"> <input type="text" id="FileReRoute_AuthenticationOptions_AllowedScopes_${index}_" 
                    name="FileReRoute.AuthenticationOptions.AllowedScopes[${index}]"
                    placeholder="Enter scope"/> <a href="#" class="btn btn-xs btn-default"><span class="glyphicon glyphicon-remove"></span></a></div>`);
        }
    };
}());

$(document).ready(function() {
    $("#button-save").on("click",
        function(e) {
            $("#form-save").submit();
        });

    $("#button-delete").on("click",
        function (e) {
            var result = confirm("Are you sure you want to delete this reroute?");
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
        }
    );
});