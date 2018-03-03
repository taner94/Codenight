
var shareid = -1;
var modalCommentBodyId = "#modal_comment_body"
$(function () {
    $("#modal_comment").on('show.bs.modal', function (e) {

        var btn = $(e.relatedTarget);
        shareid = btn.data("share-id");

        $(modalCommentBodyId).load("/Comment/ShowShareComment/" + shareid);

    })
});
function doComment(btn, e, commentid, spanid) {

    var button = $(btn);
    var mode = button.data("edit-mode");

    if (e === "edit_clicked") {
        if (!mode) {
            button.data("edit-mode", true);
            button.removeClass("btn-warning");
            button.addClass("btn-success");
            var btnSpan = button.find("span");
            btnSpan.removeClass("glyphicon-edit");
            btnSpan.addClass("glyphicon-ok");

            $(spanid).addClass("editable");
            $(spanid).attr("contenteditable", true);
            $(spanid).focus();

        }
        else {
            button.data("edit-mode", false);
            button.addClass("btn-warning");
            button.removeClass("btn-success");
            var btnSpan = button.find("span");
            btnSpan.addClass("glyphicon-edit");
            btnSpan.removeClass("glyphicon-ok");

            $(spanid).removeClass("editable");
            $(spanid).attr("contenteditable", false);

            var txt = $(spanid).text();

            $.ajax({
                method: "POST",
                url: "/Comment/Edit/" + commentid,
                data: { text: txt }
            }).done(function (data) {
                if (data.result) {
                    //yorumlar tekrar yüklenir
                    $(modalCommentBodyId).load("/Comment/ShowShareComment/" + shareid);
                }
                else {
                    alert("Yorum Güncellenemedi");
                }
            }).fail(function () {
                alert("Sunucu ile bağlantı kurulamadı.");
            });
        }
    }
    else if (e === "delete_clicked") {
        var dialog_res = confirm("Yorum silinsin mi?");
        if (!dialog_res) return false;

        $.ajax({
            method: "GET",
            url: "/Comment/Delete/" + commentid
        }).done(function (data) {

            if (data.result) {
                // yorumlar  tekrar yüklenir..
                $(modalCommentBodyId).load("/Comment/ShowShareComment/" + shareid);

            }
            else {
                alert("Yorum silinemedi.");
            }

        }).fail(function () {
            alert("Sunucu ile bağlantı kurulamadı.");
        });

    } else if (e === "new_clicked") {

        var txt = $("#new_comment_text").val();

        $.ajax({
            method: "POST",
            url: "/Comment/Create",
            data: { "CommentText": txt, "shareid": shareid }
        }).done(function (data) {
            if (data.result) {
                $(modalCommentBodyId).load("/Comment/ShowShareComment/" + shareid);
            }
            else {
                alert("Yorum eklenemedi");
            }
        }).fail(function () {
            alert("Sunucu ile bağlantı kurulamadı");
        });

    }
}

