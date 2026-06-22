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
  type: 'FoodSupply' | 'NonFoodSupply' | 'Sale' | 'Inventory' | 'Unknown';
  quantity: number;
  expirationDate: string | null;
  packagingLevel: 'New' | 'Refurbished' | 'Unsellable' | null;
  comment: string | null;
};

export type ArticleDetails = ArticleSummary & {
  allowedSaleModes: SaleMode[];
  sellableStock: number | null;
  nonSellableStock: number | null;
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
