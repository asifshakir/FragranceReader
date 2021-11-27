using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;

Word.Application wordApp = new();
wordApp.Visible = true;
wordApp.Documents.Open(@"https://d.docs.live.net/e968dcc4f80a2a1e/Islamic Books/Fragrance of Mastership/Fragrance of Mastership(volums 1-5).docx");
wordApp.Selection.GoTo(Word.WdGoToItem.wdGoToPage, Word.WdGoToDirection.wdGoToAbsolute, Name: 17);