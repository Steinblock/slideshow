
window.onload = function () {

    var grid = $("#section-grid").bootgrid({
        ajax: true,
        ajaxSettings: {
            method: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false
        },
        //url: "./.json",
        url: window.location.href,
        //post: function () {
        //    /* To accumulate custom parameter with the request object */
        //    return {
        //        id: "b0df282a-0d67-40e5-8558-c9e93b7befed"
        //    };
        //},
        //url: ".json",
        rowCount: [10, 25, 50, 100],
        //selection: true,
        //multiSelect: true,
        //rowSelect: true,
        keepSelection: true,
        formatters: {
            "link": function (column, row) {
                return '<a href="' + row.id + '">' + row.name + "</a>";
            },
            "commands": function (column, row) {
                return "<a href=\"./" + row.id + "\"> <button type=\"button\" class=\"btn btn-xs btn-default\" ><span class=\"glyphicon glyphicon-eye-open\"></span></button></a> " +
                    "<a type=\"button\" class=\"btn btn-xs btn-default command command-edit\" data-toggle=\"modal\" data-target=\"#modal-dialog-container\" data-command=\"edit\" data-load-url=\"./edit/" + row.id + "\" href=\"./edit/" + row.id + "\"><span class=\"glyphicon glyphicon-edit\"></span></a> " +
                    "<a type=\"button\" class=\"btn btn-xs btn-default command command-delete\" data-toggle=\"modal\" data-target=\"#modal-dialog-container\"  data-command=\"delete\" data-load-url=\"./delete/" + row.id + "\" href=\"./delete/" + row.id + "\"><span class=\"glyphicon glyphicon-trash\"></span></a> " +
                    "<button type=\"button\" class=\"btn btn-xs btn-default command command-up\" data-toggle=\"modal\" data-target=\"#modal-dialog-container\"  data-command=\"up\" data-url=\"./up/" + row.id + "\"><span class=\"glyphicon glyphicon-arrow-up\"></span></button> " +
                    "<button type=\"button\" class=\"btn btn-xs btn-default command command-down\" data-toggle=\"modal\" data-target=\"#modal-dialog-container\" data-command=\"down\" data-url=\"./down/" + row.id + "\"><span class=\"glyphicon glyphicon-arrow-down\"></span></button> ";
            }
        },
        labels: {
            all: "Alle",
            infos: "Datensatz {{ctx.start}} - {{ctx.end}} / {{ctx.total}}",
            loading: "Bitte warten...",
            noResults: "Keine Ergebnisse gefunden!",
            refresh: "Aktualisieren",
            search: "Suchen"
        },
    }).on("loaded.rs.jquery.bootgrid", function () {

        /* Executes after data is loaded and rendered */
        $(this).find(".command").click(function (e) {

            e.preventDefault();
            var command = $(this).attr("data-command");
            var url = $(this).attr("data-url");
            var loadurl = $(this).attr("data-load-url");

            if (url != null) {
                $("#loader").show();
                ajaxAction('get', command, url, null);
            } else if (loadurl != null) {

                $("#loader").show();

                $dialog = $($(this).attr("data-target"));
                $dialog.load(loadurl, function (data) {
                    $("#loader").hide();
                    $dialog.modal("show").on('shown.bs.modal', function () {

                        $(this).find("#btn-confirm").off("click").click(function (e) {
                            e.preventDefault();
                            ajaxCommand(command, loadurl);
                        });

                        $(this).find("#name").select();
                    }).on("hidden.bs.modal", function () {
                        $dialog.html('');
                    });

                });
            }

        });

    });

    //https://getbootstrap.com/docs/3.3/components/
    $("#section-grid-header").find('.actions.btn-group').append('<a id="btn-create" class="btn btn-primary" type="button" data-target="#modal-dialog-container" data-command="edit" data-load-url="create" href="create"><span class="icon glyphicon glyphicon-plus"></span></button>');

    $("#btn-create").click(function (e) {

        e.preventDefault();

        var command = $(this).attr("data-command");
        var loadurl = $(this).attr("data-load-url");

        $("#loader").show();
        $dialog = $($(this).attr("data-target"));
        $dialog.load(loadurl, function (data) {
            $("#loader").hide();
            $dialog.modal("show").on('shown.bs.modal', function () {

                $(this).find("#btn-confirm").off("click").click(function (e) {
                    e.preventDefault();
                    ajaxCommand(command, loadurl);
                });

                $(this).find("#name").select();
            }).on("hidden.bs.modal", function () {
                $dialog.html('');
            });

        });

    });

    function ajaxAction(type, action, url, data) {

        $.ajax({
            type: type,
            url: url,
            data: data,
            dataType: "json",
            success: function (response) {

                console.log("success:")
                console.log(response);
                $("#loader").hide();
                $("#section-grid").bootgrid('reload');
            },
            error: function (response) {
                $("#loader").hide();
                alert("Fehler bei " + action);
                console.log("error:")
                console.log(response);
            }
        });
    }

    function ajaxCommand(action, loadurl) {

        var form = $("#" + action + "-form");

        var url = form.attr('action') || loadurl;
        var type = form.attr('method') || 'get';
        var data = form.serializeArray();

        console.log(url);
        console.log(type);

        // https://www.phpflow.com/php/addedit-delete-record-using-bootgrid-php-mysql/
        $.ajax({
            type: type,
            url: url,
            data: data,
            dataType: "json",
            success: function (response) {

                console.log("success:")
                console.log(response);
                $("#modal-dialog-container").modal('hide');
                $("#section-grid").bootgrid('reload');
            },
            error: function (response) {

                //console.log("error:")
                //console.log(response);

                // clear errors
                form.find('span[data-valmsg-for]')
                    .addClass('field-validation-valid')
                    .removeClass('field-validation-error')
                    .html("");

                if (response.status === 400) {

                    // validation failed
                    console.log(response);

                    // add errors
                    if (response.responseJSON.errors) {
                        for (var key in response.responseJSON.errors) {
                            form.find('span[data-valmsg-for="' + key + '"]')
                                .addClass('field-validation-error')
                                .removeClass('field-validation-valid')
                                .html(response.responseJSON.errors[key]);
                        }
                    }

                }
                else {
                    alert("Fehler bei " + action);
                    console.log(response);
                }
            }
        });
    }

}
