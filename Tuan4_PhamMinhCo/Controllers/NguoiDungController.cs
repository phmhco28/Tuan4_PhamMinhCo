using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuan4_PhamMinhCo.Models;

namespace Tuan4_PhamMinhCo.Controllers
{
    public class NguoiDungController : Controller
    {
        // GET: NguoiDung
        MyDataDataContext data = new MyDataDataContext();

        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(FormCollection collection, KhachHang kh)
        {
            var hoten = collection["hoten"];
            var tendangnhap = collection["tendangnhap"];
            var matkhau = collection["matkhau"];
            var matkhauxacnhan = collection["matkhauxacnhan"];
            var email = collection["email"];
            var diachi = collection["diachi"];
            var dienthoai = collection["dienthoai"];
            var ngaysinh = string.Format("{0:MM/dd/yyyy}", collection["ngaysinh"]);
            var tk1 = data.KhachHangs.SingleOrDefault(s => s.email == email);
            var tk2 = data.KhachHangs.SingleOrDefault(s => s.dienthoai == dienthoai);

            if (String.IsNullOrEmpty(matkhauxacnhan))
            {
                ViewData["Error_Null_MKXN"] = "Phải nhập mật khẩu xác nhận!";
            }
            else if (tk1 != null && tk2 != null)
            {
                ViewData["Error_mail"] = "Email này đã được đăng ký !!!";
                ViewData["Error_phone"] = "Số điện thoại này đã được đăng ký !!!";
            }
            else if (tk1 != null)
            {
                ViewData["Error_mail"] = "Email này đã được đăng ký !!!";
            }
            else if (tk2 != null)
            {
                ViewData["Error_phone"] = "Số điện thoại này đã được đăng ký !!!";
            }
            else
            {
                if (!matkhau.Equals(matkhauxacnhan))
                {
                    ViewData["Error_Not_Same"] = "Mật khẩu không khớp !!!";
                }
                else
                {
                    kh.hoten = hoten;
                    kh.tendangnhap = tendangnhap;
                    kh.matkhau = matkhau;
                    kh.email = email;
                    kh.diachi = diachi;
                    kh.dienthoai = dienthoai;
                    kh.ngaysinh = DateTime.Parse(ngaysinh);

                    data.KhachHangs.InsertOnSubmit(kh);
                    data.SubmitChanges();
                    return RedirectToAction("DangNhap");
                }
            }

            return this.DangKy();
        }

        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var tendangnhap = collection["tendangnhap"];
            var matkhau = collection["matkhau"];
            KhachHang kh = data.KhachHangs.SingleOrDefault(p => p.tendangnhap == tendangnhap && p.matkhau == matkhau);
            if (kh != null)
            {
                ViewBag.ThongBao = "Chúc mừng đăng nhập thành công !!!";
                Session["TaiKhoan"] = kh;
            }
            else
            {
                ViewBag.ThongBao = "Tên đăng nhập và mật khẩu không đúng !!!";                    
            }
            return RedirectToAction("Index", "Home");
        }
    }
}