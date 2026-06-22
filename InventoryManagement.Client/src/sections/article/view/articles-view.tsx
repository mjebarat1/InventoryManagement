import type { ArticleKind, SortDirection, ArticleSummary, ArticleSortField } from 'src/api';

import { useNavigate } from 'react-router-dom';
import { useState, useEffect, useCallback } from 'react';

import Box from '@mui/material/Box';
import Chip from '@mui/material/Chip';
import Card from '@mui/material/Card';
import Alert from '@mui/material/Alert';
import Table from '@mui/material/Table';
import Button from '@mui/material/Button';
import Select from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import TableRow from '@mui/material/TableRow';
import TextField from '@mui/material/TextField';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableHead from '@mui/material/TableHead';
import InputLabel from '@mui/material/InputLabel';
import Typography from '@mui/material/Typography';
import FormControl from '@mui/material/FormControl';
import TableSortLabel from '@mui/material/TableSortLabel';
import TableContainer from '@mui/material/TableContainer';
import TablePagination from '@mui/material/TablePagination';

import { useTranslate } from 'src/locales';
import { ApiError, searchArticles } from 'src/api';
import { DashboardContent } from 'src/layouts/dashboard';

import { Iconify } from 'src/components/iconify';
import { Scrollbar } from 'src/components/scrollbar';

type Filters = { reference: string; name: string; type: '' | ArticleKind };
const initialFilters: Filters = { reference: '', name: '', type: '' };

export function ArticlesView() {
  const { t, currentLanguage } = useTranslate();
  const locale = currentLanguage.value === 'fr' ? 'fr-FR' : 'en-GB';
  const navigate = useNavigate();
  const [items, setItems] = useState<ArticleSummary[]>([]);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);
  const [totalItems, setTotalItems] = useState(0);
  const [sortBy, setSortBy] = useState<ArticleSortField>('Reference');
  const [sortDirection, setSortDirection] = useState<SortDirection>('Asc');
  const [filters, setFilters] = useState<Filters>(initialFilters);
  const [appliedFilters, setAppliedFilters] = useState<Filters>(initialFilters);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const loadArticles = useCallback((signal?: AbortSignal) => {
    setLoading(true);
    setError(null);
    return searchArticles({
      pageNumber: page + 1,
      pageSize,
      sortBy,
      sortDirection,
      type: appliedFilters.type || undefined,
      reference: appliedFilters.reference || undefined,
      name: appliedFilters.name || undefined,
    }, signal)
      .then((result) => {
        setItems(result.items);
        setTotalItems(result.totalItems);
      })
      .catch((caughtError) => {
        if (caughtError instanceof DOMException && caughtError.name === 'AbortError') return;
        setError(caughtError instanceof ApiError ? caughtError.message : t('common.error'));
      })
      .finally(() => setLoading(false));
  }, [appliedFilters, page, pageSize, sortBy, sortDirection, t]);

  useEffect(() => {
    const controller = new AbortController();
    loadArticles(controller.signal);
    return () => controller.abort();
  }, [loadArticles]);

  const applyFilters = () => {
    setPage(0);
    setAppliedFilters(filters);
  };

  const changeSort = (field: ArticleSortField) => {
    setPage(0);
    if (sortBy === field) setSortDirection((current) => current === 'Asc' ? 'Desc' : 'Asc');
    else {
      setSortBy(field);
      setSortDirection('Asc');
    }
  };

  const sortableHeader = (label: string, field: ArticleSortField) => (
    <TableCell sortDirection={sortBy === field ? (sortDirection === 'Asc' ? 'asc' : 'desc') : false}>
      <TableSortLabel active={sortBy === field} direction={sortDirection === 'Asc' ? 'asc' : 'desc'} onClick={() => changeSort(field)}>
        {label}
      </TableSortLabel>
    </TableCell>
  );

  const formatPrices = (article: ArticleSummary, field: 'vatRate' | 'priceIncludingTax') => article.prices
    .map((price) => {
      const prefix = price.saleMode ? `${t(`saleModes.${price.saleMode === 'TakeAway' ? 'takeAway' : 'onSite'}`)}: ` : '';
      const value = field === 'vatRate'
        ? new Intl.NumberFormat(locale, { style: 'percent', maximumFractionDigits: 1 }).format(price.vatRate)
        : new Intl.NumberFormat(locale, { style: 'currency', currency: 'EUR' }).format(price.priceIncludingTax);
      return `${prefix}${value}`;
    })
    .join(' / ');

  return (
    <DashboardContent>
      <Box sx={(theme) => ({ mb: 4, p: { xs: 2.5, md: 3.5 }, display: 'flex', alignItems: 'center', gap: 2, borderRadius: 3, color: 'common.white', background: `linear-gradient(135deg, ${theme.vars.palette.primary.main} 0%, ${theme.vars.palette.secondary.dark} 100%)`, boxShadow: theme.vars.customShadows.primary })}>
        <Box sx={{ flexGrow: 1 }}><Typography variant="h4">{t('articles.title')}</Typography><Typography sx={{ mt: 1, color: 'rgba(255,255,255,0.78)' }}>{t('articles.description')}</Typography></Box>
        <Button variant="contained" startIcon={<Iconify icon="mingcute:add-line" />} onClick={() => navigate('/articles/new')} sx={{ color: 'primary.dark', bgcolor: 'common.white', '&:hover': { bgcolor: 'primary.lighter' } }}>
          {t('articles.addArticle')}
        </Button>
      </Box>

      <Card sx={{ p: 2.5, mb: 3, borderTop: 3, borderTopColor: 'secondary.main' }}>
        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 1fr 220px auto' }, gap: 2 }}>
          <TextField size="small" label={t('articles.reference')} value={filters.reference} onChange={(event) => setFilters({ ...filters, reference: event.target.value })} />
          <TextField size="small" label={t('articles.name')} value={filters.name} onChange={(event) => setFilters({ ...filters, name: event.target.value })} />
          <FormControl size="small"><InputLabel id="filter-type-label">{t('articles.type')}</InputLabel><Select labelId="filter-type-label" label={t('articles.type')} value={filters.type} onChange={(event) => setFilters({ ...filters, type: event.target.value as Filters['type'] })}>
            <MenuItem value="">{t('common.all')}</MenuItem><MenuItem value="Food">{t('articleTypes.food')}</MenuItem><MenuItem value="NonFood">{t('articleTypes.nonFood')}</MenuItem>
          </Select></FormControl>
          <Button variant="outlined" onClick={applyFilters}>{t('common.search')}</Button>
        </Box>
      </Card>

      {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}
      <Card>
        <Scrollbar><TableContainer sx={{ overflow: 'unset' }}><Table sx={{ minWidth: 1100 }}>
          <TableHead><TableRow>
            {sortableHeader(t('articles.reference'), 'Reference')}{sortableHeader(t('articles.name'), 'Name')}{sortableHeader(t('articles.type'), 'Type')}{sortableHeader(t('articles.priceExcludingTax'), 'PriceExcludingTax')}
            <TableCell>{t('articles.priceIncludingTax')}</TableCell><TableCell>{t('articles.vat')}</TableCell><TableCell>{t('articles.stock')}</TableCell><TableCell>{t('articles.status')}</TableCell><TableCell>{t('articles.actions')}</TableCell>
          </TableRow></TableHead>
          <TableBody>
            {!loading && items.length === 0 ? <TableRow><TableCell colSpan={9} align="center" sx={{ py: 10 }}><Typography color="text.secondary">{t('articles.noData')}</Typography></TableCell></TableRow> : items.map((article) => (
              <TableRow hover key={article.id} sx={{ '&:hover': { bgcolor: 'primary.lighter' } }}>
                <TableCell sx={{ fontWeight: 600, color: 'primary.dark' }}>{article.reference}</TableCell><TableCell sx={{ fontWeight: 600 }}>{article.name}</TableCell><TableCell><Chip size="small" label={t(`articleTypes.${article.type === 'Food' ? 'food' : 'nonFood'}`)} color={article.type === 'Food' ? 'success' : 'secondary'} /></TableCell>
                <TableCell>{new Intl.NumberFormat(locale, { style: 'currency', currency: 'EUR' }).format(article.priceExcludingTax)}</TableCell>
                <TableCell>{formatPrices(article, 'priceIncludingTax')}</TableCell><TableCell>{formatPrices(article, 'vatRate')}</TableCell><TableCell><Chip size="small" label={article.totalStock} color={article.totalStock > 0 ? 'success' : 'default'} /></TableCell><TableCell><Chip size="small" label={t('common.unavailable')} color="warning" variant="outlined" /></TableCell>
                <TableCell><Button size="small" onClick={() => navigate(`/articles/${article.id}`)}>{t('articles.view')}</Button></TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table></TableContainer></Scrollbar>
        <TablePagination
          component="div" count={totalItems} page={page} rowsPerPage={pageSize}
          onPageChange={(_, newPage) => setPage(newPage)}
          onRowsPerPageChange={(event) => { setPageSize(Number(event.target.value)); setPage(0); }}
          rowsPerPageOptions={[10, 20, 50, 100]}
          labelRowsPerPage={t('articles.rowsPerPage')}
        />
      </Card>
    </DashboardContent>
  );
}
