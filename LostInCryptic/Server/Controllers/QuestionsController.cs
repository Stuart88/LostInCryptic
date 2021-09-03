using LostInCryptic.Server.DataContext;
using LostInCryptic.Server.Extensions;
using LostInCryptic.Shared.DbModels;
using LostInCryptic.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostInCryptic.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : ControllerBase
    {
        #region Fields

        private const string AdminPass = "treva";
        private readonly WebsiteDbContext _context;

        #endregion Fields

        #region Constructors

        public QuestionsController()
        {
            _context = new WebsiteDbContext();
        }

        #endregion Constructors

        #region Methods

        [HttpGet]
        [Route("[action]/{urlCode}/{answer}")]
        public BasicHttpResponse<Question> AnswerQuestion([FromRoute] string urlCode, [FromRoute] string answer)
        {
            try
            {
                var question = GetQuestionFromurlCode(urlCode);

                if (question == null)
                    return new BasicHttpResponse<Question>("Question not found!");

                if (question.QuestionAnswer.ToLower() != answer.ToLower())
                    return new BasicHttpResponse<Question>("Try again!") { Ok = true };

                //Correct! Return next question

                var nextQuestion = this.GetNextQuestion(question);

                if (nextQuestion == null)
                    return new BasicHttpResponse<Question>() { Ok = true, Message = "QUIZ COMPLETE!", Data = null };

                return new BasicHttpResponse<Question>(nextQuestion);
            }
            catch (Exception e)
            {
                return new BasicHttpResponse<Question>(e.ToString());
            }
        }

        [HttpPost]
        [Route("[action]")]
        public BasicHttpResponse<bool> Delete([FromBody] Question q)
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];

                if (authHeader != null && authHeader.StartsWith("Basic"))
                {
                    string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                    string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    int seperatorIndex = usernamePassword.IndexOf(':');

                    string username = usernamePassword.Substring(0, seperatorIndex);
                    string password = usernamePassword.Substring(seperatorIndex + 1);

                    if (!string.IsNullOrEmpty(password) && password == AdminPass)
                    {
                        Question deleting = _context.Questions.Find(q.Id);

                        if (deleting == null)
                        {
                            throw new Exception("Queston not found!");
                        }
                        else
                        {
                            _context.Remove(deleting);
                        }

                        _context.SaveChanges();

                        return new BasicHttpResponse<bool>(true);
                    }
                    else
                        throw new Exception("Incorrect username or password!");
                }
                else
                    throw new Exception("The authorization header is either empty or isn't Basic.");
            }
            catch (Exception e)
            {
                return new BasicHttpResponse<bool>(e.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public BasicHttpResponse<bool> AdminLogin([FromBody] string notImportant)
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];

                if (authHeader != null && authHeader.StartsWith("Basic"))
                {
                    string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                    string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    int seperatorIndex = usernamePassword.IndexOf(':');

                    string username = usernamePassword.Substring(0, seperatorIndex);
                    string password = usernamePassword.Substring(seperatorIndex + 1);

                    if (!string.IsNullOrEmpty(password) && password == AdminPass)
                    {
                        return new BasicHttpResponse<bool>(true);
                    }
                    else
                        throw new Exception("Incorrect username or password!");
                }
                else
                    throw new Exception("The authorization header is either empty or isn't Basic.");
            }
            catch (Exception e)
            {
                return new BasicHttpResponse<bool>(e.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public BasicHttpResponse<List<Question>> GetAllQuestions()
        {
            return new BasicHttpResponse<List<Question>>(_context.Questions.ToList());
        }

        [HttpGet]
        [Route("[action]/{urlCode}")]
        public BasicHttpResponse<Question> GetQuestion([FromRoute] string urlCode)
        {
            try
            {
                var question = GetQuestionFromurlCode(urlCode, getFirstIfNull: true);

                question.QuestionAnswer = "";

                return new BasicHttpResponse<Question>(question);
            }
            catch (Exception e)
            {
                return new BasicHttpResponse<Question>(e.ToString());
            }
        }

        [HttpPost]
        [Route("[action]")]
        public BasicHttpResponse<Question> Update([FromBody] Question q)
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];

                if (authHeader != null && authHeader.StartsWith("Basic"))
                {
                    string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                    string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    int seperatorIndex = usernamePassword.IndexOf(':');

                    string username = usernamePassword.Substring(0, seperatorIndex);
                    string password = usernamePassword.Substring(seperatorIndex + 1);

                    if (!string.IsNullOrEmpty(password) && password == AdminPass)
                    {
                        Question updating = _context.Questions.Find(q.Id);

                        if (updating == null)
                        {
                            q.AssignUrlCode(_context);
                            _context.Add(q);
                        }
                        else
                        {
                            updating.QuestionText = q.QuestionText;
                            updating.QuestionAnswer = q.QuestionAnswer;
                            _context.Update(updating);
                        }

                        _context.SaveChanges();

                        return new BasicHttpResponse<Question>(q);
                    }
                    else
                        throw new Exception("Incorrect username or password!");
                }
                else
                    throw new Exception("The authorization header is either empty or isn't Basic.");
            }
            catch (Exception e)
            {
                return new BasicHttpResponse<Question>(e.Message);
            }
        }

        /// <summary>
        /// Gets the next question on from the given question
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        private Question GetNextQuestion(Question question)
        {
            return _context.Questions.FirstOrDefault(q => q.Id > question.Id);
        }

        private Question GetQuestionFromurlCode(string urlCode, bool getFirstIfNull = false)
        {
            Question question = _context.Questions.FirstOrDefault(q => q.UrlCode.ToLower() == urlCode.ToLower());
            
            if(getFirstIfNull && question == null)
            {
                question = _context.Questions.First();
            }

            question.AssignQuestionNumber(_context);

            return question;
        }

        

        #endregion Methods
    }
}