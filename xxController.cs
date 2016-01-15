        #region 上传文件页
        public ActionResult UploadFile()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadFile(string str)
        {
            HttpFileCollectionBase files = Request.Files;
            bool isOK = false;
            if (files == null)
            {
                return Content("没有文件！", "text/plain");
            }
            if (!Directory.Exists(Request.MapPath("~/Upload")))
            {
                Directory.CreateDirectory(Request.MapPath("~/Upload"));
            }
            foreach (string s in files)
            {
                HttpPostedFileBase file = files[s];
                var fileName = Path.Combine(Request.MapPath("~/Upload"), Guid.NewGuid() + Path.GetFileName(file.FileName));
                try
                {
                    file.SaveAs(fileName);
                    isOK = true;
                }
                catch
                {
                    isOK = false;
                }
            }
            return Content(isOK ? "上传成功！" : "上传失败！", "text/plain");
        }
        #endregion