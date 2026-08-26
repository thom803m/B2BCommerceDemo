using B2BCommerceDemo.Core.DTOs.Import;
using CsvHelper.Configuration;

namespace B2BCommerceDemo.Infrastructure.Imports
{
    public class PurchaseOrderImportMap : ClassMap<PurchaseOrderImportDto>
    {
        public PurchaseOrderImportMap() 
        {
            Map(m => m.Sku).Name("SKU", "Sku", "sku", "SKUNumber", "SKU Number", "SKU_Number", "SkuNumber", "Sku Number", "Sku_Number", "skunumber", "sku number", "sku_number", "ProductNumber", "Product Number", "Product_Number", "productnumber", "product number", "product_number", "ProductNo.", "Product No.", "Product_No.", "Productno.", "Product no.", "Product_no.", "productno.", "product no.", "product_no.", "ItemNumber", "Item Number", "Item_Number", "Itemnumber", "Item number", "Item_number", "itemnumber", "item number", "item_number", "ItemNo.", "Item No.", "Item_No.", "Itemno.", "Item no.", "Item_no.", "itemno.", "item no.", "item_no.");
            Map(m => m.ExpectedDeliveryDate).Name("PreferredDeliveryDate", "Preferred Delivery Date", "Preferred_Delivery_Date", "Preferreddeliverydate", "Preferred delivery date", "Preferred_delivery_date", "preferreddeliverydate", "preferred delivery date", "preferred_delivery_date", "ExpectedDeliveryDate", "Expected Delivery Date", "Expected_Delivery_Date", "Expecteddeliverydate", "Expected delivery date", "Expected_delivery_date", "expecteddeliverydate", "expected delivery date", "expected_delivery_date").TypeConverterOption.Format("dd/MM/yyyy", "dd-MM-yyyy", "MM/dd/yyyy", "MM-dd-yyyy", "yyyy/MM/dd", "yyyy-MM-dd");
            Map(m => m.Quantity).Name("Quantity", "quantity", "OrderedQuantity", "Ordered Quantity", "Ordered_Quantity", "Orderedquantity", "Ordered quantity", "Ordered_quantity", "orderedquantity", "ordered quantity", "ordered_quantity").TypeConverterOption.CultureInfo(new System.Globalization.CultureInfo("da-DK"));
            Map(m => m.ReceivedQuantity).Name("ReceivedQuantity", "Received Quantity", "Received_Quantity", "Receivedquantity", "Received quantity", "Received_quantity", "receivedquantity", "received quantity", "received_quantity").TypeConverterOption.CultureInfo(new System.Globalization.CultureInfo("da-DK"));
            Map(m => m.InvoicedQuantity).Name("InvoicedQuantity", "Invoiced Quantity", "Invoiced_Quantity", "Invoicedquantity", "Invoiced quantity", "Invoiced_quantity", "invoicedquantity", "invoiced quantity", "invoiced_quantity").TypeConverterOption.CultureInfo(new System.Globalization.CultureInfo("da-DK"));
        }
    }
}

