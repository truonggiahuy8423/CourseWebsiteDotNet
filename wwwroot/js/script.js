
function toast({ title = "", message = "", type = "info", duration = 3000 }) {
  const main = document.getElementById("toast");
  if (main) {
    const toast = document.createElement("div");

    // Auto remove toast
    const autoRemoveId = setTimeout(function () {
      main.removeChild(toast);
    }, duration + 1000);

    // Remove toast when clicked
    toast.onclick = function (e) {
      if (e.target.closest(".toast__close")) {
        main.removeChild(toast);
        clearTimeout(autoRemoveId);
      }
    };

    const icons = {
      success: "fa-solid fa-circle-check",
      info: "fa-solid fa-circle-info",
      warning: "fa-solid fa-circle-exclamation",
      error: "fa-solid fa-circle-exclamation"
    };
    const icon = icons[type];
    const delay = (duration / 1000).toFixed(2);

    toast.classList.add("toast", `toast--${type}`);
    toast.style.animation = `slideInLeft ease .3s, fadeOut linear 1s ${delay}s forwards`;

    toast.innerHTML = `
      <div class="">
          <i class="toast__icon ${icon}"></i>
      </div>
      <div class="toast__body">
          <h3 class="toast__title">${title}</h3>
          <p class="toast__msg">${message}</p>
      </div>
      <div class="">
          <i class="toast__close fa-solid fa-circle-xmark"></i>
      </div>
    `;
    main.appendChild(toast);
  }
}

function showSuccessToast() {
  toast({
    title: 'Thành công',
    message: 'Bạn đã thành công',
    type: 'success',
    duration: 5000
  });
}

function showInfoToast() {
  toast({
    title: 'Thông tin',
    message: '',
    type: 'info',
    duration: 5000
  });
}

function showWarningToast() {
  toast({
    title: 'Cảnh báo',
    message: '',
    type: 'warning',
    duration: 5000
  });
}

function showErrorToast() {
  toast({
    title: 'Thất bại',
    message: 'Có lỗi xảy ra, vui lòng liên hệ quản trị viên',
    type: 'error',
    duration: 5000
  });
}

function loadingEffect(state) {
  if (state) {
    $('')
  } else {

  }
}

function setDisable() {

}

function loadingEffect(state) {
  if (state) {
    $('body').append("<div class='loading-effect'><i class='fa-solid fa-spinner loading-icon'></i></div>");
  } else {
    $('.loading-effect').remove();
  }
}

$(document).ready(function () {
  setInterval(function () {
    var url = window.location.hostname;

    // Tạo đối tượng URL từ URL hiện tại

    // Lấy tên miền
    var currentDate = new Date();

    // Tạo các biến để lưu giữ giờ, phút, giây
    var hours = currentDate.getHours();
    var minutes = currentDate.getMinutes();
    var seconds = currentDate.getSeconds();

    // Định dạng các giá trị để đảm bảo đều có 2 chữ số
    hours = (hours < 10) ? "0" + hours : hours;
    minutes = (minutes < 10) ? "0" + minutes : minutes;
    seconds = (seconds < 10) ? "0" + seconds : seconds;

    // Lấy ngày, tháng, năm
    var year = currentDate.getFullYear();
    var month = (currentDate.getMonth() + 1); // Tháng bắt đầu từ 0
    var day = currentDate.getDate();

    // Định dạng ngày, tháng, năm để đảm bảo đều có 2 chữ số
    month = (month < 10) ? "0" + month : month;
    day = (day < 10) ? "0" + day : day;

    // Tạo chuỗi theo định dạng "Y-m-d H:i:s"
    var formattedDate = year + "-" + month + "-" + day + " " + hours + ":" + minutes + ":" + seconds;

    $.ajax({
      url: `${window.location.protocol}//${window.location.hostname}/Admin/CoursesController/active`,
      method: 'POST',
      contentType: 'text', // Đặt kiểu dữ liệu của yêu cầu là JSON
      data: null,
      success: function (response) {
        console.log(response);
       }
    })
  }, 10000);
})

async function chooseUserFile() {
  return new Promise(function(resolve, reject) {
    loadingEffect(true);
    $.ajax({
      url: `${window.location.protocol}//${window.location.hostname}/Admin/CoursesController/getChooseUserFileForm`,
      contentType: "",
      dataType: "text",
      data: "",
      success: function(response) {
        loadingEffect(false);
        $(`body`).append(response);
        setTimeout(function() {
          $(`.get-file-form`).css('opacity', '1');
         }, 100);
         $(document).ready(function() {
          $(`.get-file-form__cancel-btn`).click(function() {
            $(`.form-container--file`).remove();
            resolve(false);
          });
          $(`.get-file-form__save-btn`).click(function() {
            let dom = $(`.file-item[ischosen="true"]`);
            let file_id = dom.attr('value');
            let fileName =  removeNewlines(tachChuoiCham(dom.text()).fileName).trim();
            let extension =  removeNewlines(tachChuoiCham(dom.text()).extension).trim();

            // setTimeout(function() {
              $(`.form-container--file`).remove();
            
            resolve({
              id_tep_tin_tai_len: file_id,
              ten_tep: fileName,
              extension: extension
            });
          });
         })
      },
      error: function() {
  
      }
    })
  })
}
function tachChuoiCham(chuoi) {
  var index = chuoi.indexOf('.');

  if (index !== -1) {
      var truocCham = chuoi.substring(0, index);
      var sauCham = chuoi.substring(index + 1);
      return { fileName: truocCham, extension: sauCham };
  } else {
      return { fileName: chuoi, extension: '' };
  }
}

function removeNewlines(inputString) {
  // Sử dụng biểu thức chính quy để thay thế tất cả các ký tự \n thành chuỗi rỗng
  var resultString = inputString.replace(/\n/g, '');

  return resultString;
}