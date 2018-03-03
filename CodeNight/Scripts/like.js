
$(function () {

    var shareids = [];

    $("div[data-share-id]").each(function (i, e) {

        shareids.push($(e).data("share-id"));
    });
    $.ajax({
        method: "POST",
        url: "/Share/GetLiked",
        data: { ids: shareids }
    }).done(function (data) {

        if (data.result != null && data.result.length > 0) {
            for (var i = 0; i < data.result.length; i++) {
                var id = data.result[i];
                var likedShare = $("div[data-share-id=" + id + "]");
                var btn = likedShare.find("button[data-liked]")
                var span = btn.find("span.like-hand");

                btn.data("liked", true);
                span.removeClass("glyphicon-hand-right");
                span.addClass("glyphicon-heart");
                btn.removeClass("btn-default");
                btn.addClass("btn-primary");
            }
        }

    }).fail(function () {

    });

    $("button[data-liked]").click(function () {

        var btn = $(this);
        var liked = btn.data("liked");
        var shareid = btn.data("share-id");
        var spanLike = btn.find("span.like-hand");
        var spanCount = btn.find("span.like-count");

        $.ajax({
            method: "POST",
            url: "/Share/SetLikeState",
            data: { "shareid": shareid, "liked": !liked }
        }).done(function () {

            if (data.hasError) {
                alert(data.errorMessage);
            } else {
                liked = !liked;
                btn.data("liked", liked);
                spanCount.text(data.result);
                location.reload();

                spanLike.removeClass("glyphicon-hand-right");
                spanLike.removeClass("glyphicon-heart");
                btn.removeClass("btn-primary");
                btn.removeClass("btn-default");

                if (liked) {
                    spanLike.addClass("glyphicon-heart");
                    btn.addClass("btn-primary");

                } else {
                    spanLike.addClass("glyphicon-hand-right");
                    btn.addClass("btn-default");

                }

            }

        }).fail(function () {
            alert("Sunucuyla bağlantı kurulamadı");
        });

    });
    $(function () {
        $(document).on("click", "#btn1", function () {
            location.reload();

        });
    });
});
