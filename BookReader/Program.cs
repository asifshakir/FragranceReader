using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace BookReader
{
    internal class Program
    {
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var wd = new Word.Application();
            wd.Visible = true;
            //try
            {
                var location = Assembly.GetExecutingAssembly().Location;
                var folder = Path.GetDirectoryName(location);
                string filename = "Fragrance_English_vol_1_5.docx";
                var filepath = Path.Combine(folder, "Books", filename);
                wd.Documents.Open(filepath, ReadOnly: true);
                var currentLine = wd.ActiveDocument
                                    .GoTo(
                                        Word.WdGoToItem.wdGoToPage,
                                        Word.WdGoToDirection.wdGoToAbsolute,
                                        Name: 17);
                currentLine.Select();
                wd.Selection.EndKey(Word.WdUnits.wdStory, Word.WdMovementType.wdExtend);
                var documentParas = wd.Selection.Paragraphs;
                var paraReferences = new List<Word.Paragraph>();
                foreach (Word.Paragraph para in documentParas)
                {
                    paraReferences.Add(para);
                }
                var paras = new List<Para>();

                var testCtr = 0;
                foreach(Word.Paragraph para in paraReferences)
                {
                    string htmlText = string.Empty;
                    string paraText = para.Range.Text.Trim();
                    string paraStyle = para.get_Style().NameLocal;
                    para.Range.Select();
                    wd.Selection.Copy();
                    htmlText = ReadClipboardHtml();
                    htmlText = htmlText ?? paraText;
                    paras.Add(new Para
                    {
                        HtmlText = htmlText,
                        Text = paraText,
                        Style = paraStyle
                    });
                    testCtr++;
                    //if (testCtr > 50) break;
                }
                wd.ActiveDocument.Close(false);
                wd.Quit();

                sw.Stop();
                Console.WriteLine("Total Time Taken={0}", sw.Elapsed);

                var topicCtr = 1;

                var traditions = new List<Tradition>();
                Tradition tradition = null;
                string previousStyle = null;
                foreach (var para in paras)
                {
                    if (para.Style == "Heading 1")
                    {
                        if (para.Text != "Bibliography")
                        {
                            if (tradition != null)
                            {
                                traditions.Add(tradition);
                            }
                            previousStyle = "title";
                            tradition = new Tradition()
                            {
                                Title = para.Text,
                                Arabic = new List<string>(),
                                English = new List<string>(),
                                References = new List<string>(),
                                Notes = new List<string>(),
                            };
                            topicCtr++;
                        }
                    } else
                    {
                        if(para.Style == "arabic")
                        {
                            previousStyle = "arabic";
                            tradition.Arabic.Add(para.Text);
                        }
                        else if (para.Style == "Normal" || para.Style == "hadees")
                        {
                            previousStyle = "english";
                            tradition.English.Add(para.HtmlText);
                        }
                        else if (para.Style == "Heading 2" && para.Text.StartsWith("Note"))
                        {
                            previousStyle = "notes";
                        }
                        else if (previousStyle == "notes" && para.Style == "indent")
                        {
                            tradition.Notes.Add(para.HtmlText);
                        }
                        else if (para.Style == "Heading 2" && para.Text.StartsWith("Reference"))
                        {
                            previousStyle = "reference";
                        }
                        else if (previousStyle == "reference" && para.Style == "indent")
                        {
                            tradition.References.Add(para.Text);
                        }
                    }
                }
                if(tradition != null) traditions.Add(tradition);

                var DB = new DB.BookDbContext();

                DB.TraditionTranslations.RemoveRange(DB.TraditionTranslations);
                DB.TraditionReferences.RemoveRange(DB.TraditionReferences);
                DB.Traditions.RemoveRange(DB.Traditions);

                var tNo = 1;
                foreach (var tr in traditions)
                {
                    if (tr.English.Count > 0)
                    {
                        var t = new DB.Tradition
                        {
                            BookId = 1,
                            TraditionNo = tNo,
                            Text = String.Join("\n", tr.English),
                            Notes = String.Join("\n", tr.Notes)
                        };
                        DB.Traditions.Add(t);
                        DB.SaveChanges();
                        tNo++;

                        if (tr.Arabic.Count > 0)
                        {
                            var t_tr = new DB.TraditionTranslation
                            {
                                TraditionId = t.Id,
                                LanguageId = 1,
                                Text = String.Join("\n", tr.Arabic)
                            };
                            DB.TraditionTranslations.Add(t_tr);
                            DB.SaveChanges();
                        }

                        if (tr.References.Count > 0) 
                        {
                            foreach (var r in tr.References)
                            {
                                string volume = null;
                                string page = null;
                                string hadith = null;
                                string bookname = r;

                                if (r.Contains(','))
                                {
                                    var bookParts = r.Split(',');
                                    bookname = bookParts[0];
                                    bookname = bookname.Replace("\t", "");
                                    string pattern = "^[0-9]+\\.";
                                    bookname = Regex.Replace(bookname, pattern, "").Trim();

                                    var partCtr = 1;
                                    if (bookParts.Length > partCtr)
                                    {
                                        if (bookParts[partCtr].Trim().StartsWith("vol"))
                                        {
                                            volume = bookParts[partCtr].Replace("vol. ", "");
                                        }
                                        if (bookParts[partCtr].Trim().StartsWith("p"))
                                        {
                                            page = bookParts[partCtr].Replace("p. ", "");
                                        }
                                        if (bookParts[partCtr].Trim().StartsWith("H"))
                                        {
                                            hadith = bookParts[partCtr].Replace("H. ", "");
                                        }
                                        if (bookParts[partCtr].Trim().StartsWith("No"))
                                        {
                                            hadith = bookParts[partCtr].Replace("No. ", "");
                                        }
                                    }
                                    partCtr++;
                                    if (bookParts.Length > partCtr)
                                    {
                                        if (bookParts[partCtr].Trim().StartsWith("vol"))
                                        {
                                            volume = bookParts[partCtr].Replace("vol. ", "");
                                        }
                                        if (bookParts[partCtr].Trim().StartsWith("p"))
                                        {
                                            page = bookParts[partCtr].Replace("p. ", "");
                                        }
                                        if (bookParts[partCtr].Trim().StartsWith("H"))
                                        {
                                            hadith = bookParts[partCtr].Replace("H. ", "");
                                        }
                                        if (bookParts[partCtr].Trim().StartsWith("No"))
                                        {
                                            hadith = bookParts[partCtr].Replace("No. ", "");
                                        }
                                    }
                                    partCtr++;
                                    if (bookParts.Length > partCtr)
                                    {
                                        if (bookParts[partCtr].Trim().StartsWith("vol"))
                                        {
                                            volume = bookParts[partCtr].Replace("vol. ", "");
                                        }
                                        if (bookParts[partCtr].Trim().StartsWith("p"))
                                        {
                                            page = bookParts[partCtr].Replace("p. ", "");
                                        }
                                        if (bookParts[partCtr].Trim().StartsWith("H"))
                                        {
                                            hadith = bookParts[partCtr].Replace("H. ", "");
                                        }
                                        if (bookParts[partCtr].Trim().StartsWith("No"))
                                        {
                                            hadith = bookParts[partCtr].Replace("No. ", "");
                                        }
                                    }
                                }

                                var r_tr = new DB.TraditionReference
                                {
                                    TraditionId = t.Id,
                                    Reference = r,
                                    Book = bookname,
                                    Volume = volume,
                                    Page = page,
                                    Hadith = hadith
                                };
                                DB.TraditionReferences.Add(r_tr);
                            }
                            DB.SaveChanges();
                        }
                    }
                }
            }
            //catch (Exception ex)
            {
                //Console.WriteLine($"{ex.Message}, {ex.StackTrace}");
            }
            Console.ReadKey();
        }

        public static string ReadClipboardHtml()
        {
            string html = null;
            if (Clipboard.ContainsText(TextDataFormat.Html))
            {
                var sd = Clipboard.GetData(DataFormats.Html);
                if (sd == null)
                {
                    return null;
                }
                var s = sd.ToString();
                var st = s.IndexOf("<!--StartFragment-->") + 21;
                var en = s.IndexOf("<!--EndFragment-->");
                html = s.Substring(st, en - st).Trim();

                html = html.Replace("\r\n", " ");
                html = html.Replace('\r', ' ');
                html = html.Replace('\n', ' ');
                html = html.Replace("<o:p>", "");
                html = html.Replace("</o:p>", "");
                html = html.Replace(" lang=EN-IN", "");
                string pattern = " style='.*?'";
                html = Regex.Replace(html, pattern, "");

                HtmlAgilityPack.HtmlDocument agiDoc = new HtmlAgilityPack.HtmlDocument();
                agiDoc.LoadHtml(html);
                var spans = agiDoc.DocumentNode.SelectNodes("//span");
                if (spans != null)
                {
                    foreach (var span in spans)
                    {
                        var classes = span.GetClasses();
                        if (classes.Count() == 0)
                        {
                            span.Name = "empty";
                        }
                    }

                    html = agiDoc.DocumentNode.InnerHtml;
                }

                var p = agiDoc.DocumentNode.SelectNodes("//p");
                if (p != null)
                {
                    foreach (var pT in p)
                    {
                        if (pT.HasClass("MsoNormal"))
                        {
                            pT.RemoveClass("MsoNormal");
                        }

                    }

                    html = agiDoc.DocumentNode.InnerHtml;
                }

                html = html.Replace("<empty>", "");
                html = html.Replace("</empty>", "");
            }
            return html;
        }
    }

    public class Para
    {
        public string Text { get; set; }
        public string HtmlText { get; set; }
        public string Style { get; set; }

    }

    public class Tradition
    {
        public string Title { get; set; }
        public List<string> Arabic { get; set; }
        public List<string> English { get; set; }
        public List<string> Notes { get; set; }
        public List<string> References { get; set; }
    }
}
