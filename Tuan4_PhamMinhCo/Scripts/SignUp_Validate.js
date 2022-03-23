

$(document).ready(function () {
	$(".validate").validate({
		rules: {
			hoten: "required",
			diachi: {
				required: true,
				minlength: 2
			}
		},
		messages: {
			hoten: "Vui lòng nhập họ",
			diachi: {
				required: "Vui lòng nhập địa chỉ",
				minlength: "Địa chỉ ngắn vậy, chém gió ah?"
			}
		}
	});
});