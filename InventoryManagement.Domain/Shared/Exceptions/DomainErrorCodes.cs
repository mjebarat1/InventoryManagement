namespace InventoryManagement.Domain.Shared.Exceptions;

public static class DomainErrorCodes
{
    public const string ArticleRequired = "article.required";
    public const string ArticleNameRequired = "article.name.required";
    public const string ArticleInactive = "article.inactive";
    public const string ArticleTypeUnknown = "article.type.unknown";
    public const string ArticleReferenceRequired = "article.reference.required";
    public const string ArticleReferenceInvalid = "article.reference.invalid";
    public const string ArticleReferenceAlreadyExists = "article.reference.already_exists";
    public const string ArticleSaleModesRequired = "article.sale_modes.required";
    public const string ArticleSaleModesForbidden = "article.sale_modes.forbidden";
    public const string PaginationPageNumberInvalid = "pagination.page_number.invalid";
    public const string PaginationPageSizeInvalid = "pagination.page_size.invalid";
    public const string StockBucketReferenceInvalid = "stock_bucket.reference.invalid";
    public const string StockBucketReferenceAlreadyExists = "stock_bucket.reference.already_exists";
    public const string StockBucketSearchInvalid = "stock_bucket.search.invalid";
    public const string StockBucketExpirationRequiredForFood = "stock_bucket.expiration_date.required_for_food";
    public const string StockBucketExpirationForbiddenForNonFood = "stock_bucket.expiration_date.forbidden_for_non_food";
    public const string StockBucketPackagingForbiddenForFood = "stock_bucket.packaging.forbidden_for_food";
    public const string StockBucketPackagingRequiredForNonFood = "stock_bucket.packaging.required_for_non_food";
    public const string QuantityMustBeNonNegative = "quantity.must_be_non_negative";
    public const string QuantityMustBePositive = "quantity.must_be_positive";
    public const string AmountMustBeNonNegative = "amount.must_be_non_negative";
    public const string VatRateMustBeNonNegative = "vat_rate.must_be_non_negative";
    public const string VatRateInvalid = "vat_rate.invalid";
    public const string StockInsufficient = "stock.insufficient";
    public const string SaleModeNotAllowed = "sale_mode.not_allowed";
    public const string SaleModeUnknown = "sale_mode.unknown";
    public const string SaleModeRequired = "sale_mode.required";
    public const string SaleModeForbidden = "sale_mode.forbidden";
    public const string InventoryAdjustmentRequired = "inventory.adjustment.required";
    public const string InventoryNoDifference = "inventory.no_difference";
    public const string InventoryExistingBucketsRequired = "inventory.existing_buckets.required";
    public const string InventoryNewBucketsRequired = "inventory.new_buckets.required";
    public const string InventorySelectionRequired = "inventory.selection.required";
    public const string InventoryDuplicateExistingBucket = "inventory.existing_bucket.duplicate";
    public const string InventoryBucketNotInArticle = "inventory.bucket.not_in_article";
    public const string InventoryDuplicateNewReference = "inventory.new_reference.duplicate";
}
