using System;
using System.Collections.Generic;
using System.Linq;
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
            List<GioHang> listGioHang = Laygiohang();
            foreach (var item in listGioHang)
            {
                var sanpham = data.Saches.FirstOrDefault(p => p.masach == item.masach);
                sanpham.soluongton -= item.iSoluong;
                UpdateModel(sanpham);
                data.SubmitChanges();
            }
            return RedirectToAction("Giohang");
        }
    }
}