import { apiRequest } from './http-client';

export type ArticleKind = 'Food' | 'NonFood';
export type SaleMode = 'TakeAway' | 'OnSite';
export type ArticleSortField = 'Reference' | 'Name' | 'Type' | 'PriceExcludingTax';
export type SortDirection = 'Asc' | 'Desc';

export type ArticlePrice = {
  saleMode: SaleMode | null;
  vatRate: number;
  priceIncludingTax: number;
};

export type ArticleSummary = {
  id: string;
  reference: string;
  name: string;
  type: ArticleKind;
  priceExcludingTax: number;
  prices: ArticlePrice[];
  totalStock: number;
};

export type StockMovement = {
  id: string;
  createdAt: string;
  type: 'Supply' | 'Sale' | 'Inventory' | 'Unknown';
  quantityDelta: number;
  saleMode: SaleMode | null;
  soldQuantity: number | null;
  unitPriceExcludingTax: number | null;
  unitPriceIncludingTax: number | null;
  vatRate: number | null;
  totalExcludingTax: number | null;
  totalIncludingTax: number | null;
  comment: string | null;
  lines: StockMovementLine[];
};

export type StockMovementLine = {
  id: string;
  stockBucketId: string;
  stockBucketReference: string;
  bucketType: ArticleKind;
  expirationDate: string | null;
  packagingLevel: 'New' | 'Refurbished' | 'Unsellable' | null;
  quantityDelta: number;
  quantityBefore: number | null;
  quantityAfter: number | null;
};

export type StockBucketStatus = 'Empty' | 'Sellable' | 'Expired' | 'Unsellable';

export type StockBucket = {
  id: string;
  reference: string;
  createdAt: string;
  type: ArticleKind;
  expirationDate: string | null;
  packagingLevel: 'New' | 'Refurbished' | 'Unsellable' | null;
  physicalQuantity: number;
  sellableQuantity: number;
  status: StockBucketStatus;
};

export type ArticleDetails = ArticleSummary & {
  allowedSaleModes: SaleMode[];
  sellableStock: number;
  nonSellableStock: number;
  buckets: StockBucket[];
  movements: StockMovement[];
};

export type SearchArticlesRequest = {
  pageNumber: number;
  pageSize: number;
  sortBy: ArticleSortField;
  sortDirection: SortDirection;
  type?: ArticleKind;
  reference?: string;
  name?: string;
};

export type PagedArticles = {
  items: ArticleSummary[];
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
};

type CreatedArticle = { id: string };
type RecordedSupply = { movementId: string; bucketId: string };
type RecordedSale = { movementId: string; soldQuantity: number };
type RecordedInventory = { movementId: string; adjustedBucketCount: number; createdBucketCount: number };

export type RecordSupplyRequest = {
  stockBucketReference: string;
  quantity: number;
  expirationDate: string | null;
  packagingLevel: 'New' | 'Refurbished' | 'Unsellable' | null;
};

export type RecordInventoryRequest = {
  comment: string | null;
  existingBuckets: Array<{
    stockBucketId: string;
    countedQuantity: number;
  }>;
  newBuckets: Array<{
    reference: string;
    countedQuantity: number;
    expirationDate: string | null;
    packagingLevel: 'New' | 'Refurbished' | 'Unsellable' | null;
  }>;
};

export type RecordSaleRequest = {
  quantity: number;
  saleMode: SaleMode | null;
};

export function searchArticles(request: SearchArticlesRequest, signal?: AbortSignal) {
  return apiRequest<PagedArticles>('/api/articles/search', {
    method: 'POST',
    body: JSON.stringify(request),
    signal,
  });
}

export function getArticleById(articleId: string, signal?: AbortSignal) {
  return apiRequest<ArticleDetails>(`/api/articles/${articleId}`, { signal });
}

export function searchStockBuckets(articleId: string, referenceDigits: string, signal?: AbortSignal) {
  return apiRequest<StockBucket[]>(`/api/articles/${articleId}/stock-buckets/search`, {
    method: 'POST',
    body: JSON.stringify({ referenceDigits }),
    signal,
  });
}

export function createFoodArticle(request: {
  reference: string;
  name: string;
  priceExcludingTax: number;
  saleModes: SaleMode[];
}) {
  return apiRequest<CreatedArticle>('/api/articles/food', {
    method: 'POST',
    body: JSON.stringify(request),
  });
}

export function createNonFoodArticle(request: {
  reference: string;
  name: string;
  priceExcludingTax: number;
}) {
  return apiRequest<CreatedArticle>('/api/articles/non-food', {
    method: 'POST',
    body: JSON.stringify(request),
  });
}

export function recordSupply(articleId: string, request: RecordSupplyRequest) {
  return apiRequest<RecordedSupply>(`/api/articles/${articleId}/supplies`, {
    method: 'POST',
    body: JSON.stringify(request),
  });
}

export function recordSale(articleId: string, request: RecordSaleRequest) {
  return apiRequest<RecordedSale>(`/api/articles/${articleId}/sales`, {
    method: 'POST',
    body: JSON.stringify(request),
  });
}

export function recordInventory(articleId: string, request: RecordInventoryRequest) {
  return apiRequest<RecordedInventory>(`/api/articles/${articleId}/inventories`, {
    method: 'POST',
    body: JSON.stringify(request),
  });
}
