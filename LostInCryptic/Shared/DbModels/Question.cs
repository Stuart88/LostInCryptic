using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using static LostInCryptic.Shared.Enums.Enums;

namespace LostInCryptic.Shared.DbModels
{
    [Table("Questions")]
    public class Question
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique, NotNull]
        public string UrlCode { get; set; }

        [NotNull]
        public string QuestionText { get; set; }
        
        [NotNull]
        public string QuestionAnswer
        {
            get { return answer; }
            set { this.answer = new string(value.Where(c => c == ' '  || char.IsLetterOrDigit(c)).ToArray()) ; }
        }

        private string answer;

        /// <summary>
        /// This is set based on how many db entries come before it. 
        /// </summary>
        [Ignore] // NOTE: this is currently not working. Instead it is set on OnModelCreating
        public int Number { get; set; } = 1;

    }
}
