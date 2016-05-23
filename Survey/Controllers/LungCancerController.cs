using DB;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Survey.Controllers
{
    public class LungCancerController : Controller
    {
        // GET: LungCancer
        public async Task<ActionResult> Index()
        {
            var document = new BsonDocument
            {
                { "patient_id",1}
            };
            await MongoDatabase.Patients.InsertOneAsync(document);

            return View();
        }

        public async Task<ActionResult> CancerName()
        {
            var filter = Builders<BsonDocument>.Filter.Eq("patient_id", 1);
            var result = await MongoDatabase.Patients.Find(filter).ToListAsync();
            return View();
        }

        public async Task<ActionResult> PathologyType(string CancerName)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("patient_id", 1);
            var update = Builders<BsonDocument>.Update
                .Set("Cancername", CancerName);
            var result = await MongoDatabase.Patients.UpdateOneAsync(filter, update);

            return View();
        }

        public async Task<ActionResult> Period(string PathologyType)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("patient_id", 1);
            var update = Builders<BsonDocument>.Update
                .Set("PathologyType", PathologyType);
            var result = await MongoDatabase.Patients.UpdateOneAsync(filter, update);
            return View();
        }

        public async Task<ActionResult> Therapies(string PeriodT,string PeriodN,string PeriodM,string Clinical, string Simple,string PeriodNone)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("patient_id", 1);
            var update = Builders<BsonDocument>.Update
                .Set("Period.TNM.T", PeriodT)
                .Set("Period.TNM.N", PeriodN)
                .Set("Period.TNM.M", PeriodM)
                .Set("Period.Clinical", Clinical)
                .Set("Period.Simple", Simple)
                .Set("Period.Unknown", PeriodNone);
            var result = await MongoDatabase.Patients.UpdateOneAsync(filter, update);
            return View();
        }

        public ActionResult Recovery(bool? a,bool? b,bool? c,bool? d,bool? e,bool? f,bool? g,bool? h,bool? i,bool? j,bool? k,bool? l,bool? m,bool? n,bool? o, bool? p,bool? q,bool? r,bool? s, bool? t, bool? u,bool? v)
        {
            if (a.HasValue|| b.HasValue || c.HasValue || d.HasValue || e.HasValue || f.HasValue || g.HasValue || h.HasValue || i.HasValue || j.HasValue || k.HasValue || l.HasValue || m.HasValue || n.HasValue || o.HasValue || p.HasValue || q.HasValue  )
            {
                HttpContext.Cache["hualiao"]=true;
            }
            else
            {
                HttpContext.Cache.Remove("hualiao");
            }

            if(r.HasValue||s.HasValue||t.HasValue||u.HasValue)
            {
                HttpContext.Cache["shoushu"] = true;
            }
            else
            {
                HttpContext.Cache.Remove("shoushu");
            }

            if (v.HasValue)
            {
                HttpContext.Cache["fangliao"] = true;
            }
            else
            {
                HttpContext.Cache.Remove("fangliao");
            }

            return View();
        }

        public ActionResult OtherDisease()
        {
            return View();
        }

        public ActionResult Psychological()
        {
            return View();
        }

        public ActionResult Report(bool? b,bool? d,bool? e,bool? f,bool? g,bool? h)
        {
            if (b.HasValue || d.HasValue || e.HasValue || f.HasValue || g.HasValue || h.HasValue)
            {
                ViewBag.a = "请您调整心态，你可能对肿瘤存在误解，肿瘤本身并不向您想想的那样可怕，只要调整好心态，积极配合医生治疗，治愈或长期生存是完全有可能的。";
            }
            else
            {
                ViewBag.a = " 您目前的心态是正确，请您继续保持好心情，肿瘤并不可怕，更要对自己和医学有信心";
            }

            if (HttpContext.Cache.Get("hualiao")!=null)
            {
                ViewBag.b = "<p>化疗建议</p><p>（1）治疗一年内，每3到4个月检查一次颈胸部CT和头颅MRI</p><p>（2）治疗一年后，每半年检查一次颈胸部CT和头颅MRI</p><p>（3）治疗后满五年，每年检查一次颈胸部CT和头颅MRI</p>";
                ViewBag.f = "<p>请参考《部分肿瘤常用化疗药物及副作用》表<p/>";
            }
            else
            {
                ViewBag.b = "<p>无<p/>";
                ViewBag.f = "<p></p>";
            }

            if (HttpContext.Cache.Get("shoushu") != null)
            {
                ViewBag.c = "术后恢复"
                    +"<p>1、术后短期内要经常做“腹式呼吸”和“有效咳嗽”，锻炼自己的肺功能，使没有被切除的肺能够顺利膨胀开；</p>"
                    + "<p>2、如果平躺呼吸不适，可以采取半卧位；（如果选择的是“一侧全肺切除”，则需要叮嘱患者：）切莫向健侧侧躺着！</p>"
                    + "<p>3、注意保持切口处的干净清爽，避免感染发生；</p>"
                    + "<p>4、患者所在的房间内空气不要太干燥，适当可以使用空气加湿器，并且要每日开窗通风；</p>"
                    + "<p>5、术后体质一般较虚弱，多食用鸡蛋、牛奶、甲鱼和水果蔬菜等有营养的食物，增强身体抵抗力，同时避免受凉。</p>";
            }
            else
            {
                ViewBag.c = "<p><p/>";
            }

            if (HttpContext.Cache.Get("fangliao") != null)
            {
                ViewBag.d = "放疗恢复"
                    + "<p>1、放疗照射部位的皮肤要保持清洁，若皮肤出现痛痒，可以涂一些“鱼肝油软膏”，若症状持续或加重，请与肿瘤科医生联系；</p>"
                    + "<p>2、放疗照射部位要防止摩擦、暴晒以及碱性较大的肥皂的刺激，防止皮肤破溃感染；</p>"
                    + "<p>3、一旦有了局部破溃感染，应及时使用全身和局部抗菌消炎药；若破溃已经结痂，结痂不能用手抠除，应让其自然脱落；</p>"
                    + "<p>4、放疗病人往往或出现口唇干燥、舌红少苔、味嗅觉减弱等现象，应多服用一些滋阴生津的饮品，如：藕汁、绿豆汤、冬瓜汤、西瓜汁等；还有多吃一些鱼、奶、蜂蜜、蔬菜水果等。</ p>";

                ViewBag.e = "<p>放疗病人往往或出现口唇干燥、舌红少苔、味嗅觉减弱等现象，应多服用一些滋阴生津的饮品，如：藕汁、绿豆汤、冬瓜汤、西瓜汁等；还有多吃一些鱼、奶、蜂蜜、蔬菜水果等。</p>";
                
            }
            else
            {
                ViewBag.d = "<p><p/>";
                ViewBag.e = "<p></p>";
                
            }


            return View();
        }
    }
}