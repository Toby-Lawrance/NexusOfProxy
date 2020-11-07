using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Fonts;
using UglyToad.PdfPig.Writer;

namespace NexusofProxyLib
{
    public class PDFCreator
    {
        private PdfDocumentBuilder builder;
        private PdfPageBuilder pageBuilder;
        private int cardCount = 0;
        private ScryfallEndPoint sep;

        private bool EmptyPage = false;

        private int CardWidth = 180;
        private int CardHeight = 252;
        public double Spacing = 2.0;

        public PDFCreator(ScryfallEndPoint s, bool empty = false)
        {
            sep = s;
            MakePDF();
            EmptyPage = empty;
            AddPage();
        }

        public void SavePDF(string path)
        {
            var content = builder.Build();
            File.WriteAllBytes(path, content);
        }

        private void MakePDF()
        {
            builder = new PdfDocumentBuilder();
        }

        public void AddPage()
        {
            if (EmptyPage)
            {
                //Prevents blank first page
                if (pageBuilder is not null) 
                {
                    builder.AddPage(PageSize.A4);
                }
            }
            pageBuilder = builder.AddPage(PageSize.A4);
            cardCount = 0;
        }

        public void AddCard(string cardName,string set,bool back)
        {
            if (cardCount == 9)
            {
                AddPage();
            }

            var fileName = sep.MakeRequest(cardName,set,back);
            if (!File.Exists(fileName))
            {
                return;
            }

            var s = new FileStream(fileName, FileMode.Open);


            var width = pageBuilder.PageSize.Width;
            var height = pageBuilder.PageSize.Height;

            double CardWidthSpaced = 180 + Spacing;
            double CardHeightSpaced = 252 + Spacing;

            double LeftMargin = (width - (CardWidthSpaced * 3)) / 2.0; //24.5;
            double TopMargin = (height - (CardHeightSpaced * 3)) / 2.0; //40;

            PdfRectangle placement =
                new PdfRectangle(LeftMargin, TopMargin, CardWidth + LeftMargin, CardHeight + TopMargin);

            int xTransCount = cardCount % 3;
            int yTransCount = (cardCount / 3) % 3;

            placement = placement.Translate(xTransCount * CardWidthSpaced, yTransCount * CardHeightSpaced);

            pageBuilder.AddPng(s, placement);
            cardCount++;
            s.Close();
        }
    }
}