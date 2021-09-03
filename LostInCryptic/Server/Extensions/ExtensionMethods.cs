using LostInCryptic.Server.DataContext;
using LostInCryptic.Shared.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostInCryptic.Server.Extensions
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Assigns a unique urlCode to the given question
        /// </summary>
        /// <param name="q"></param>
        /// <param name="db"></param>
        public static void AssignUrlCode(this Question q, WebsiteDbContext db)
        {
            Random r = new Random();
            int maxId = db.UrlCodes.Max(w => w.Id);
            
            while (string.IsNullOrEmpty(q.UrlCode) || db.Questions.Any(quest => quest.UrlCode == q.UrlCode))
            {
                q.UrlCode = db.UrlCodes.Find(r.Next(1, maxId + 1))?.Value ?? null;
            }
        }

        /// <summary>
        /// Assign question number based on how many come before it (needs to be this way because number might change if a prior question is deleted)
        /// </summary>
        /// <param name="q"></param>
        /// <param name="db"></param>
        public static void AssignQuestionNumber(this Question q, WebsiteDbContext db)
        {
            q.Number = db.Questions.Count(_q => _q.Id <= q.Id);
        }
    }
}
