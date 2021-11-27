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
            try
            {
                wd.Documents.Open(@"https://d.docs.live.net/e968dcc4f80a2a1e/Islamic Books/Fragrance of Mastership/Fragrance of Mastership(volums 1-5).docx", ReadOnly: true);
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
                    if (testCtr > 10) break;
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

                var xl = new Excel.Application();
                xl.Visible = true;
                xl.Workbooks.Add();
                var rowCtr = 2;
                foreach(var tr in traditions)
                {
                    var colCtr = 0;
                    xl.ActiveSheet.Range[$"A{rowCtr}"].Value = tr.Title;
                    var text = string.Empty;
                    foreach(var a in tr.Arabic)
                    {
                        text += text == string.Empty ? string.Empty : "\n";
                        text += a;
                    }
                    colCtr++;
                    xl.ActiveSheet.Range[$"A{rowCtr}"].Offset[0, colCtr].Value = text;
                    text = string.Empty;
                    foreach (var a in tr.English)
                    {
                        text += text == string.Empty ? string.Empty : "\n";
                        text += a;
                    }
                    colCtr++;
                    xl.ActiveSheet.Range[$"A{rowCtr}"].Offset[0, colCtr].Value = text;
                    text = string.Empty;
                    foreach (var a in tr.Notes)
                    {
                        text += text == string.Empty ? string.Empty : "\n";
                        text += a;
                    }
                    colCtr++;
                    xl.ActiveSheet.Range[$"A{rowCtr}"].Offset[0, colCtr].Value = text;
                    text = string.Empty;
                    foreach (var a in tr.References)
                    {
                        text += text == string.Empty ? string.Empty : "\n";
                        text += a;
                    }
                    colCtr++;
                    xl.ActiveSheet.Range[$"A{rowCtr}"].Offset[0, colCtr].Value = text;
                    text = string.Empty;
                    rowCtr++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open file, {ex.Message}");
            }
            Console.ReadKey();
        }

        public static string ReadClipboardHtml()
        {
            string html = null;
            if (Clipboard.ContainsText(TextDataFormat.Html))
            {
                var s = Clipboard.GetData(DataFormats.Html).ToString();
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
