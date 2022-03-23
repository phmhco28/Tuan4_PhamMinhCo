using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Tuan4_PhamMinhCo.Models;

namespace Tuan4_PhamMinhCo.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GiaHang

        MyDataDataContext data = new MyDataDataContext();
        public ActionResult Index()
        {
            return View();
        }

        public List<GioHang> Laygiohang()
        {
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if (listGioHang == null)
            {
                listGioHang = new List<GioHang>();
                Session["GioHang"] = listGioHang;
            }
            return listGioHang;
        }

        public ActionResult Themgiohang(int id, string strURL)
        {
            List<GioHang> listGioHang = Laygiohang();
            GioHang sanpham = listGioHang.Find(s => s.masach == id);
            if (sanpham == null)
            {
                sanpham = new GioHang(id);
                listGioHang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }

        private int Tongsoluong()
        {
            int tsl = 0;
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if (listGioHang != null)
            {
                tsl = listGioHang.Sum(p => p.iSoluong);
            }
            return tsl;
        }

        private int Tongsoluongsanpham()
        {
            int tsl = 0;
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if (listGioHang != null)
            {
                tsl = listGioHang.Count();
            }
            return tsl;
        }

        private double Tongtien()
        {
            double tt = 0;
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if (listGioHang != null)
            {
                tt = listGioHang.Sum(p => p.dthanhtien);
            }
            return tt;
        }

        public ActionResult GioHang()
        {
            List<GioHang> listGioHang = Laygiohang();
            ViewBag.Tongsoluong = Tongsoluong();
            ViewBag.Tongtien = Tongtien();
            ViewBag.Tongsoluongsanpham = Tongsoluongsanpham();
            return View(listGioHang);
        }
        
        public ActionResult GioHangPartial()
        {
            ViewBag.Tongsoluong = Tongsoluong();
            ViewBag.Tongtien = Tongtien();
            ViewBag.Tongsoluongsanpham = Tongsoluongsanpham();
            return PartialView();
        }

        public ActionResult Xoagiohang(int id)
        {
            List<GioHang> listGioHang = Laygiohang();
            GioHang sanpham = listGioHang.SingleOrDefault(p => p.masach == id);
            if (sanpham != null)
            {
                listGioHang.RemoveAll(s => s.masach == id);
                return RedirectToAction("GioHang");
            }
            return RedirectToAction("GioHang");
        }

        private bool checkQuantity(int sl, GioHang sp)
        {
            List<GioHang> listGioHang = Laygiohang();
            var checkProduct = data.Saches.FirstOrDefault(p => p.masach == sp.masach);
            if (sl > checkProduct.soluongton)
            {
                return false;
            }
            return true;
        }
        
        public ActionResult Capnhatgiohang(int id, FormCollection collection)
        {
            int soluongOder = int.Parse(collection["txtSoluong"].ToString());
            List<GioHang> listGioHang = Laygiohang();
            GioHang sanpham = listGioHang.SingleOrDefault(p => p.masach == id);
            if (checkQuantity(soluongOder, sanpham) == false)
            {
                TempData["value"] = "NotEnough";
                return RedirectToAction("GioHang");
            }
            else if (sanpham != null)
            {
                sanpham.iSoluong = soluongOder;
            }
            return RedirectToAction("GioHang");
        }


        public ActionResult Xoatatcagiohang()
        {
            List<GioHang> listGioHang = Laygiohang();
            listGioHang.Clear();
            return RedirectToAction("GioHang");
        }

        public ActionResult Dathang()
        {           
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Sach");
            }
            List<GioHang> listGioHang = Laygiohang();
            ViewBag.Tongsoluong = Tongsoluong();
            ViewBag.Tongtien = Tongtien();
            ViewBag.Tongsoluongsanpham = Tongsoluongsanpham();
            return View(listGioHang);
        }

        [HttpPost]
        public ActionResult Dathang(FormCollection collection)
        {
            DonHang dh = new DonHang();
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            Sach s = new Sach();

            List<GioHang> gh = Laygiohang();
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["ngaygiao"]);
            if (DateTime.Parse(ngaygiao) <= DateTime.Now)
            {
                ViewData["Error_Deliverdate"] = "Ngày giao hàng phải lớn hơn ngày đặt hàng !!!";
            }
            else
            {
                dh.makh = kh.makh;
                dh.ngaydat = DateTime.Now;
                dh.ngaygiao = DateTime.Parse(ngaygiao);
                dh.giaohang = false;
                dh.thanhtoan = false;
                data.DonHangs.InsertOnSubmit(dh);
                data.SubmitChanges();

                foreach (var item in gh)
                {
                    ChiTietDonHang ctdh = new ChiTietDonHang();
                    ctdh.madon = dh.madon;
                    ctdh.masach = item.masach;
                    ctdh.soluong = item.iSoluong;
                    ctdh.gia = (decimal)item.giaban;
                    s = data.Saches.SingleOrDefault(p => p.masach == item.masach);
                    s.soluongton -= ctdh.soluong;
                    data.SubmitChanges();
                    data.ChiTietDonHangs.InsertOnSubmit(ctdh);
                }
                data.SubmitChanges();
                Session["Giohang"] = null;
                //Send Email to Customer
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("store.confirmmail@gmail.com", "BookStore");
                    var receiverEmail = new MailAddress(kh.email, "Receiver");
                    var password = "password"; //lahcbhn
                    var sub = "XAC_NHAN_DON_HANG";
                    var body = "Tên khách hàng: " + kh.hoten + "\n" +
                                "Địa chỉ: " + kh.diachi + "\n" +
                                "Số điện thoại: " + kh.dienthoai + "\n" +
                                "Ngày đặt hàng: " + DateTime.Now.ToShortDateString() + "\n" +
                                "Ngày giao hàng: " + ngaygiao + "\n";
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = sub,
                        Body = body
                    })
                    {
                        smtp.Send(mess);
                    }
                }
                ViewBag.Error = "Some Error";
                return RedirectToAction("XacNhanDonHang", "GioHang");
            }
            return this.Dathang();            
        }

        public ActionResult XacNhanDonHang()
        {
            return View();
        }
    }
}