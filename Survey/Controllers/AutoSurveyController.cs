using DB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Survey.Controllers
{
    public class AutoSurveyController : Controller
    {
        // GET: AutoSurvey
        public async Task<ActionResult> Index()
        {
            var questions = await MongoDatabase.Tables.Find(new BsonDocument()).ToListAsync();
            ViewBag.questions = questions.ToJson<List<BsonDocument>>();
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.id = DateTime.Now.Ticks;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CreateSurvey()
        {
            try
            {
                Request.InputStream.Position = 0;
                string input = new StreamReader(Request.InputStream).ReadToEnd();
                BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(input);
                await MongoDatabase.Tables.InsertOneAsync(document);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
            return Json(new { success = true });
        }


        public async Task<ActionResult> Edit(string id)
        {
            var questionsFilter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var question = await MongoDatabase.Tables.Find(questionsFilter).FirstOrDefaultAsync();
            ViewBag.question = question.ToJson<BsonDocument>();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> SaveEdit()
        {
            try
            {
                Request.InputStream.Position = 0;
                string input = await new StreamReader(Request.InputStream).ReadToEndAsync();
                BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(input);
                var filter = Builders<BsonDocument>.Filter.Eq("_id", document["_id"].AsString);
                var update = Builders<BsonDocument>.Update
                    .Set("title", document["title"].AsString)
                    .Set("type", document["type"].AsString)
                    .Set("application", document["application"].AsString);
                await MongoDatabase.Tables.UpdateOneAsync(filter, update);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public async Task<ActionResult> Survey(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var result = await MongoDatabase.Tables.Find(filter).FirstOrDefaultAsync();

            ViewBag.id = id;
            ViewBag.title = result["title"].AsString;
            ViewBag.type = result["type"].AsString;
            ViewBag.application = result["application"].AsString;
            ViewBag._id = DateTime.Now.Ticks;
            int i = 0;
            string schema = string.Empty;
            schema += '{';
            foreach (var n in result["questions"].AsBsonArray)
            {
                schema += "question" + (i++) + ":{";

                string type = n["type"].AsString;
                schema += string.Format("type: '{0}',", type);

                if (type == "Radio" || type == "Checkboxes")
                {
                    var options = n["options"].AsBsonArray;
                    schema += "options: [";

                    foreach (var option in options)
                    {
                        schema += "'" + option.AsString + "',";
                    }
                    schema = schema.TrimEnd(',');
                    schema += "],";
                }

                string title = n["title"].AsString;
                schema += string.Format("title: '{0}'", title);

                schema += "},";
            }
            schema = schema.TrimEnd(',');
            schema += "}";

            ViewBag.schema = schema;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> SaveSurvey()
        {
            try
            {
                Request.InputStream.Position = 0;
                string input = new StreamReader(Request.InputStream).ReadToEnd();
                BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(input);
                await MongoDatabase.Rows.InsertOneAsync(document);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
            return Json(new { success = true });
        }

        public async Task<ActionResult> Show(string id)
        {
            var answersFilter = Builders<BsonDocument>.Filter.Eq("surveyId", id);
            var answers = await MongoDatabase.Rows.Find(answersFilter).ToListAsync();
            ViewBag.answers = answers.ToJson<List<BsonDocument>>();
            return View();
        }

        public async Task<ActionResult> Record(string id)
        {
            var answerFilter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var answer = await MongoDatabase.Rows.Find(answerFilter).FirstOrDefaultAsync();
            ViewBag.answer = answer.ToJson<BsonDocument>();
            var questionFilter = Builders<BsonDocument>.Filter.Eq("_id", answer["surveyId"].AsString);
            var question = await MongoDatabase.Tables.Find(questionFilter).FirstOrDefaultAsync();
            ViewBag.question = question.ToJson<BsonDocument>();
            return View();
        }

        public async Task<ActionResult> Analyze(string id)
        {
            var answerFilter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var answer = await MongoDatabase.Rows.Find(answerFilter).FirstOrDefaultAsync();
            var questionFilter = Builders<BsonDocument>.Filter.Eq("_id", answer["surveyId"].AsString);
            var question = await MongoDatabase.Tables.Find(questionFilter).FirstOrDefaultAsync();
            IList<int> sum = new List<int>();
            for (int n = 0; n < question["questions"].AsBsonArray.Count; n++)
            {
                if (question["questions"][n]["type"] == "Radio")
                {
                    var o = question["questions"][n]["options"].AsBsonArray;
                    var q = answer["answers"]["question" + n];
                    var i = o.IndexOf(q);
                    sum.Add(i);
                }
            }

            var result = sum.Average();
            if (result == 0)
            {
                ViewBag.result = "完全康复";
            }
            else if (result < 1 && result > 0)
            {
                ViewBag.result = "完全缓解";
            }
            else if (1 <= result && result < 2)
            {
                ViewBag.result = "显效";
            }
            else if (result >= 2 && result < 3)
            {
                ViewBag.result = "好转";
            }
            else
            {
                ViewBag.result = "无效";
            }

            return View();
        }
    }
}