using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Configurations;
using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.Extensions.Options;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public class InvoiceService : IInvoiceService
	{
		private RestaurantDbContext DbContext { get; }
		private InvoiceConfiguration InvoiceConfig { get; }
		private string InvoicePath { get; }

		private PdfDocument doc;
		private XGraphics gfx;
		private PdfPage page;
		private XPdfFontOptions fontOptions;
		private XFont defaultFont;
		private XPen defaultPen;

		public InvoiceService(RestaurantDbContext dbContext, IOptions<InvoiceConfiguration> invoiceConfig)
		{
			this.DbContext = dbContext;
			this.InvoiceConfig = invoiceConfig.Value;
			InvoicePath = $"../{InvoiceConfig.Directory}";
			Directory.CreateDirectory(InvoicePath);
		}

		public async Task<Stream> GetInvoice(int id)
		{
			var invoice = await DbContext.Invoices.FindAsync(id);

			if (invoice == null)
				throw new RestaurantNotFoundException("Nincs ilyen számla!");

			return File.OpenRead($"{InvoicePath}/{invoice.Path}");
		}

		public async Task<int> CreateInvoice(InvoiceCreationModel model)
		{
			var creationTime = DateTime.Now;
			var invoiceId = await SaveInvoice(model, creationTime);
			GeneratePDF(model, creationTime, invoiceId);
			return invoiceId;
		}

		private async Task<int> SaveInvoice(InvoiceCreationModel model, DateTime creationTime)
		{
			int? billingDataId = null;
			if (!string.IsNullOrEmpty(model.CustomerName) && !string.IsNullOrEmpty(model.CustomerAddress))
			{
				var billingData = DbContext.Add(new BillingData()
				{
					Name = model.CustomerName,
					TaxNumber = model.CustomerTaxNumber,
					Address = model.CustomerAddress,
					PhoneNumber = model.CustomerPhoneNumber,
					Email = model.CustomerEmail
				});
				await DbContext.SaveChangesAsync();
				billingDataId = billingData.Entity.Id;
			}

			var invoice = DbContext.Add(new Invoice()
			{
				OrderSessionId = model.OrderSession.Id,
				CreationTime = creationTime,
				BillingDataId = billingDataId,
				PaymentMethod = model.PaymentMethod
			});

			await DbContext.SaveChangesAsync();

			invoice.Entity.Path = $"invoice{invoice.Entity.Id}.pdf";

			await DbContext.SaveChangesAsync();

			return invoice.Entity.Id;
		}

		private void GeneratePDF(InvoiceCreationModel model, DateTime CreationDate, int invoiceId)
		{
			doc = new PdfDocument($"{InvoicePath}/invoice{invoiceId}.pdf");
			doc.Info.Title = "Számla";

			AddPage();

			XFont titleFont = new XFont(InvoiceConfig.FontFamily, InvoiceConfig.FontSize + 4, XFontStyle.Bold, fontOptions);
			XFont smallFont = new XFont(InvoiceConfig.FontFamily, InvoiceConfig.FontSize - 2, XFontStyle.Regular, fontOptions);
			XFont boldFont = new XFont(InvoiceConfig.FontFamily, InvoiceConfig.FontSize, XFontStyle.Bold, fontOptions);

			int x = InvoiceConfig.Margin;

			//Title
			WriteText(x, 20, "Számla", titleFont);

			DrawLine(x, 30, page.Width - InvoiceConfig.Margin, 30);

			WriteText(page.Width - 140, 20, $"Azonosító: R/{invoiceId}", smallFont);

			//Restaurant data
			WriteText(x, 45, "Kibocsátó:", smallFont);

			WriteText(x, 70, InvoiceConfig.Restaurant.Name, titleFont);

			WriteText(x, 90, InvoiceConfig.Restaurant.Address);

			WriteText(x, 110, $"Adószám: {InvoiceConfig.Restaurant.TaxNumber}");

			WriteText(x, 130, $"Tel.: {InvoiceConfig.Restaurant.PhoneNumber}");

			WriteText(x, 150, $"E-mail: {InvoiceConfig.Restaurant.Email}");

			//Customer data
			int y = 45;
			if (!string.IsNullOrEmpty(model.CustomerName) && !string.IsNullOrEmpty(model.CustomerAddress))
			{
				int x2 = (int)Math.Round(page.Width / 2) + InvoiceConfig.Margin;
				WriteText(x2, y, "Vevő:", smallFont);
				y = 70;
				WriteText(x2, y, model.CustomerName, titleFont);
				y = 90;
				WriteText(x2, y, model.CustomerAddress);
				y = 110;
				if (!string.IsNullOrEmpty(model.CustomerTaxNumber))
				{
					WriteText(x2, y, $"Adószám: {model.CustomerTaxNumber}");
					y += 20;
				}
				if (!string.IsNullOrEmpty(model.CustomerPhoneNumber))
				{
					WriteText(x2, y, $"Tel.: {model.CustomerPhoneNumber}");
					y += 20;
				}
				if (!string.IsNullOrEmpty(model.CustomerEmail))
				{
					WriteText(x2, y, $"E-mail: {model.CustomerEmail}");
					y += 20;
				}

				DrawLine(x2 - InvoiceConfig.Margin, 30, x2 - InvoiceConfig.Margin, 170);
			}
			DrawLine(x, 170, page.Width - InvoiceConfig.Margin, 170);

			//Header end

			WriteText(x, 190, $"Fizetési mód: {EnumHelper.GetDescription(model.PaymentMethod)}");

			//Table header
			int tablePageUnitX = (int)Math.Floor((page.Width - InvoiceConfig.Margin * 2) / 12);
			WriteText(InvoiceConfig.Margin, 220, "Tétel", boldFont);
			WriteText(InvoiceConfig.Margin + 6 * tablePageUnitX, 220, "Mennyiség", boldFont);
			WriteText(InvoiceConfig.Margin + 8 * tablePageUnitX, 220, "Egységár", boldFont);
			WriteText(InvoiceConfig.Margin + 10 * tablePageUnitX, 220, "Összeg", boldFont);
			DrawLine(x, 230, page.Width - InvoiceConfig.Margin, 230);
			y = 250;
			//Order items (table content)
			model.OrderSession.Orders.ForEach(order =>
			{
				order.Items.ForEach(oi =>
				{
					WriteText(InvoiceConfig.Margin, y, oi.MenuItem.Name, maxWidth: 6 * tablePageUnitX);
					WriteText(InvoiceConfig.Margin + 6 * tablePageUnitX, y, oi.Quantity.ToString(), maxWidth: 2 * tablePageUnitX);
					WriteText(InvoiceConfig.Margin + 8 * tablePageUnitX, y, $"{oi.MenuItem.Price} Ft", maxWidth: 2 * tablePageUnitX);
					WriteText(InvoiceConfig.Margin + 10 * tablePageUnitX, y, $"{oi.MenuItem.Price * oi.Quantity} Ft", maxWidth: 2 * tablePageUnitX);
					y += 20;
					if (y > page.Height)
					{
						AddPage();
						y = 20;
					}
				});
			});

			if (model.FullPrice != model.FinalPrice)
			{

				DrawLine(x, y, page.Width - InvoiceConfig.Margin, y);
				y += 20;
				WriteText(InvoiceConfig.Margin + 10 * tablePageUnitX, y, $"{model.FullPrice} Ft", boldFont);
				y += 20;
				DrawLine(x, y, page.Width - InvoiceConfig.Margin, y);
				y += 20;

				//Voucher
				if (!string.IsNullOrEmpty(model.VoucherCode) && model.VoucherDiscountAmount != null)
				{
					WriteText(InvoiceConfig.Margin, y, model.VoucherCode, maxWidth: 6 * tablePageUnitX);
					WriteText(InvoiceConfig.Margin + 10 * tablePageUnitX, y, $"-{model.VoucherDiscountAmount} Ft", maxWidth: 2 * tablePageUnitX);
					y += 20;
				}

				//Loyalty points
				if(model.RedeemedLoyaltyPoints > 0)
				{
					WriteText(InvoiceConfig.Margin, y, "Hűségpontok", maxWidth: 6 * tablePageUnitX);
					WriteText(InvoiceConfig.Margin + 6 * tablePageUnitX, y, model.RedeemedLoyaltyPoints.ToString(), maxWidth: 2 * tablePageUnitX);
					WriteText(InvoiceConfig.Margin + 10 * tablePageUnitX, y, $"-{model.RedeemedLoyaltyPoints} Ft", maxWidth: 2 * tablePageUnitX);
					y += 20;
				}
			}

			//Sum
			DrawLine(x, y, page.Width - InvoiceConfig.Margin, y);
			y += 20;
			WriteText(InvoiceConfig.Margin + 9 * tablePageUnitX, y, $"Összesen: {model.FinalPrice} Ft", boldFont);
			y += 40;
			WriteText(x, y, $"AP: {InvoiceConfig.Restaurant.APNumber}");
			y += 20;
			WriteText(x, y, $"Kelt: {CreationDate:yyyy. MM. dd. HH:mm:ss}");

			doc.Close();
		}

		private void AddPage()
		{
			page = doc.AddPage();
			gfx = XGraphics.FromPdfPage(page);
			gfx.MUH = PdfFontEncoding.Unicode;
			fontOptions = new XPdfFontOptions(PdfFontEncoding.Unicode);
			defaultFont = new XFont(InvoiceConfig.FontFamily, InvoiceConfig.FontSize, XFontStyle.Regular, fontOptions);
			defaultPen = new XPen(XColor.FromKnownColor(XKnownColor.Black));
		}

		private void WriteText(double x, double y, string text, XFont font = null, int? maxWidth = null)
		{
			int measurementErrorPadding = 10;
			var usedFont = font ?? defaultFont;
			var textWidth = GetTextWidth(text, (int)usedFont.Size, usedFont.FontFamily.Name, usedFont.Style);
			gfx.DrawString(text, usedFont, XBrushes.Black, new XRect(x, y, Math.Min(textWidth + measurementErrorPadding, maxWidth ?? int.MaxValue), 0));
		}

		private void DrawLine(double x1, double y1, double x2, double y2, XPen pen = null)
		{
			gfx.DrawLine(pen ?? defaultPen, x1, y1, x2, y2);
		}

		private int GetTextWidth(string text, int fontSize, string fontFamily, XFontStyle fontStyle)
		{
			var usedFontStyle = (fontStyle == XFontStyle.Bold) ? FontStyle.Bold : FontStyle.Regular;	//Note: we only use these 2
			using Graphics graphics = Graphics.FromImage(new Bitmap(1, 1));
			SizeF size = graphics.MeasureString(text, new Font(fontFamily, fontSize, usedFontStyle, GraphicsUnit.Point));
			return (int)Math.Round(size.Width);
		}
	}
}
