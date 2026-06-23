import type { SaleMode, StockBucket, ArticlePrice, StockMovement, ArticleDetails } from 'src/api';

import { Fragment, useState, useEffect, useCallback } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';

import Box from '@mui/material/Box';
import Chip from '@mui/material/Chip';
import Card from '@mui/material/Card';
import Alert from '@mui/material/Alert';
import Table from '@mui/material/Table';
import Dialog from '@mui/material/Dialog';
import Button from '@mui/material/Button';
import Select from '@mui/material/Select';
import Checkbox from '@mui/material/Checkbox';
import MenuItem from '@mui/material/MenuItem';
import Snackbar from '@mui/material/Snackbar';
import Collapse from '@mui/material/Collapse';
import TableRow from '@mui/material/TableRow';
import TextField from '@mui/material/TextField';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableHead from '@mui/material/TableHead';
import IconButton from '@mui/material/IconButton';
import InputLabel from '@mui/material/InputLabel';
import Typography from '@mui/material/Typography';
import FormControl from '@mui/material/FormControl';
import DialogTitle from '@mui/material/DialogTitle';
import Autocomplete from '@mui/material/Autocomplete';
import DialogContent from '@mui/material/DialogContent';
import DialogActions from '@mui/material/DialogActions';
import TableContainer from '@mui/material/TableContainer';
import InputAdornment from '@mui/material/InputAdornment';
import FormControlLabel from '@mui/material/FormControlLabel';
import CircularProgress from '@mui/material/CircularProgress';

import { DashboardContent } from 'src/layouts/dashboard';
import { useTranslate, translateApiError } from 'src/locales';
import { recordSale, recordSupply, updateArticle, getArticleById, recordInventory, searchStockBuckets } from 'src/api';

import { Iconify } from 'src/components/iconify';

const formatMoney = (value: number, locale: string) => new Intl.NumberFormat(locale, { style: 'currency', currency: 'EUR' }).format(value);
const formatVat = (value: number, locale: string) => new Intl.NumberFormat(locale, { style: 'percent', maximumFractionDigits: 1 }).format(value);
const formatDateOnly = (value: string, locale: string) => new Intl.DateTimeFormat(locale).format(new Date(`${value}T00:00:00`));
const sanitizeReferenceDigits = (value: string) => value.replace(/\D/g, '').slice(0, 13);

type InventoryExistingLine = { bucket: StockBucket; countedQuantity: string };
type InventoryNewLine = {
  key: string;
  referenceDigits: string;
  countedQuantity: string;
  expirationDate: string;
  packagingLevel: 'New' | 'Refurbished' | 'Unsellable';
};

export function ArticleDetailsView() {
  const { id } = useParams();
  const { t, currentLanguage } = useTranslate();
  const locale = currentLanguage.value === 'fr' ? 'fr-FR' : 'en-GB';
  const location = useLocation();
  const navigate = useNavigate();
  const [article, setArticle] = useState<ArticleDetails | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [notification, setNotification] = useState<{ severity: 'success' | 'error'; message: string } | null>(
    location.state?.articleCreated ? { severity: 'success', message: t('articleCreate.success') } : null
  );
  const [supplyOpen, setSupplyOpen] = useState(false);
  const [supplyReferenceDigits, setSupplyReferenceDigits] = useState('');
  const [supplyQuantity, setSupplyQuantity] = useState('');
  const [supplyExpirationDate, setSupplyExpirationDate] = useState('');
  const [supplyPackaging, setSupplyPackaging] = useState<'New' | 'Refurbished' | 'Unsellable'>('New');
  const [supplyError, setSupplyError] = useState<string | null>(null);
  const [supplySubmitting, setSupplySubmitting] = useState(false);
  const [saleOpen, setSaleOpen] = useState(false);
  const [saleQuantity, setSaleQuantity] = useState('');
  const [saleMode, setSaleMode] = useState<SaleMode | ''>('');
  const [saleError, setSaleError] = useState<string | null>(null);
  const [saleSubmitting, setSaleSubmitting] = useState(false);
  const [inventoryOpen, setInventoryOpen] = useState(false);
  const [inventoryBucketToAdd, setInventoryBucketToAdd] = useState<StockBucket | null>(null);
  const [inventorySearchDigits, setInventorySearchDigits] = useState('');
  const [inventorySearchOptions, setInventorySearchOptions] = useState<StockBucket[]>([]);
  const [inventorySearchLoading, setInventorySearchLoading] = useState(false);
  const [inventoryExistingLines, setInventoryExistingLines] = useState<InventoryExistingLine[]>([]);
  const [inventoryNewLines, setInventoryNewLines] = useState<InventoryNewLine[]>([]);
  const [inventoryComment, setInventoryComment] = useState('');
  const [inventoryError, setInventoryError] = useState<string | null>(null);
  const [inventorySubmitting, setInventorySubmitting] = useState(false);
  const [editOpen, setEditOpen] = useState(false);
  const [editName, setEditName] = useState('');
  const [editPrice, setEditPrice] = useState('');
  const [editSaleModes, setEditSaleModes] = useState<SaleMode[]>([]);
  const [editError, setEditError] = useState<string | null>(null);
  const [editSubmitting, setEditSubmitting] = useState(false);

  const loadArticle = useCallback(async (signal?: AbortSignal) => {
    if (!id) return;
    const result = await getArticleById(id, signal);
    setArticle(result);
    setError(null);
  }, [id]);

  useEffect(() => {
    const controller = new AbortController();
    loadArticle(controller.signal)
      .catch((caughtError) => {
        if (caughtError instanceof DOMException && caughtError.name === 'AbortError') return;
        setError(translateApiError(caughtError, t));
      });
    return () => controller.abort();
  }, [loadArticle, t]);

  useEffect(() => {
    if (!inventoryOpen || !id || inventorySearchDigits.length < 9) {
      setInventorySearchOptions([]);
      setInventorySearchLoading(false);
      return undefined;
    }

    const controller = new AbortController();
    const timer = window.setTimeout(() => {
      setInventorySearchLoading(true);
      searchStockBuckets(id, inventorySearchDigits, controller.signal)
        .then(setInventorySearchOptions)
        .catch((caughtError) => {
          if (caughtError instanceof DOMException && caughtError.name === 'AbortError') return;
          setInventoryError(translateApiError(caughtError, t));
        })
        .finally(() => {
          if (!controller.signal.aborted) setInventorySearchLoading(false);
        });
    }, 300);

    return () => {
      window.clearTimeout(timer);
      controller.abort();
    };
  }, [id, inventoryOpen, inventorySearchDigits, t]);

  const openSupplyDialog = () => {
    setSupplyReferenceDigits('');
    setSupplyQuantity('');
    setSupplyExpirationDate('');
    setSupplyPackaging('New');
    setSupplyError(null);
    setSupplyOpen(true);
  };

  const submitSupply = async () => {
    if (!id || !article) return;
    if (!/^\d{13}$/.test(supplyReferenceDigits)) {
      setSupplyError(t('supply.validation.reference'));
      return;
    }
    const quantity = Number(supplyQuantity);
    if (!Number.isInteger(quantity) || quantity <= 0) {
      setSupplyError(t('supply.validation.quantity'));
      return;
    }
    if (article.type === 'Food' && !supplyExpirationDate) {
      setSupplyError(t('supply.validation.expirationDate'));
      return;
    }

    setSupplySubmitting(true);
    setSupplyError(null);
    try {
      await recordSupply(id, {
        stockBucketReference: `ref-lot-${supplyReferenceDigits}`,
        quantity,
        expirationDate: article.type === 'Food' ? supplyExpirationDate : null,
        packagingLevel: article.type === 'NonFood' ? supplyPackaging : null,
      });
      setSupplyOpen(false);
      setNotification({ severity: 'success', message: t('supply.success') });

      try {
        await loadArticle();
      } catch {
        setNotification({ severity: 'error', message: t('supply.refreshError') });
      }
    } catch (caughtError) {
      const message = translateApiError(caughtError, t);
      setSupplyError(message);
      setNotification({ severity: 'error', message });
    } finally {
      setSupplySubmitting(false);
    }
  };

  const openInventoryDialog = () => {
    setInventoryBucketToAdd(null);
    setInventorySearchDigits('');
    setInventorySearchOptions([]);
    setInventoryExistingLines([]);
    setInventoryNewLines([]);
    setInventoryComment('');
    setInventoryError(null);
    setInventoryOpen(true);
  };

  const addExistingInventoryBucket = () => {
    if (!inventoryBucketToAdd) return;
    if (inventoryExistingLines.some((line) => line.bucket.id === inventoryBucketToAdd.id)) {
      setInventoryError(t('inventory.validation.duplicateBucket'));
      return;
    }
    setInventoryExistingLines((current) => [
      ...current,
      { bucket: inventoryBucketToAdd, countedQuantity: String(inventoryBucketToAdd.physicalQuantity) },
    ]);
    setInventoryBucketToAdd(null);
    setInventorySearchDigits('');
    setInventorySearchOptions([]);
    setInventoryError(null);
  };

  const addNewInventoryBucket = () => {
    setInventoryNewLines((current) => [
      ...current,
      {
        key: `new-${Date.now()}-${current.length}`,
        referenceDigits: '',
        countedQuantity: '',
        expirationDate: '',
        packagingLevel: 'New',
      },
    ]);
    setInventoryError(null);
  };

  const submitInventory = async () => {
    if (!id || !article) return;
    if (inventoryExistingLines.length === 0 && inventoryNewLines.length === 0) {
      setInventoryError(t('inventory.validation.noSelection'));
      return;
    }

    const existingQuantitiesValid = inventoryExistingLines.every((line) => {
      const value = Number(line.countedQuantity);
      return Number.isInteger(value) && value >= 0;
    });
    if (!existingQuantitiesValid) {
      setInventoryError(t('inventory.validation.countedQuantity'));
      return;
    }

    const existingReferences = new Set(article.buckets.map((bucket) => bucket.reference));
    const newReferences = inventoryNewLines.map((line) => `ref-lot-${line.referenceDigits}`);
    if (inventoryNewLines.some((line) => !/^\d{13}$/.test(line.referenceDigits))) {
      setInventoryError(t('inventory.validation.reference'));
      return;
    }
    if (new Set(newReferences).size !== newReferences.length || newReferences.some((reference) => existingReferences.has(reference))) {
      setInventoryError(t('inventory.validation.duplicateReference'));
      return;
    }
    if (inventoryNewLines.some((line) => !Number.isInteger(Number(line.countedQuantity)) || Number(line.countedQuantity) <= 0)) {
      setInventoryError(t('inventory.validation.newQuantity'));
      return;
    }
    if (article.type === 'Food' && inventoryNewLines.some((line) => !line.expirationDate)) {
      setInventoryError(t('inventory.validation.expirationDate'));
      return;
    }

    const hasExistingDifference = inventoryExistingLines.some(
      (line) => Number(line.countedQuantity) !== line.bucket.physicalQuantity
    );
    if (!hasExistingDifference && inventoryNewLines.length === 0) {
      setInventoryError(t('inventory.validation.noDifference'));
      return;
    }

    setInventorySubmitting(true);
    setInventoryError(null);
    try {
      await recordInventory(id, {
        comment: inventoryComment.trim() || null,
        existingBuckets: inventoryExistingLines.map((line) => ({
          stockBucketId: line.bucket.id,
          countedQuantity: Number(line.countedQuantity),
        })),
        newBuckets: inventoryNewLines.map((line) => ({
          reference: `ref-lot-${line.referenceDigits}`,
          countedQuantity: Number(line.countedQuantity),
          expirationDate: article.type === 'Food' ? line.expirationDate : null,
          packagingLevel: article.type === 'NonFood' ? line.packagingLevel : null,
        })),
      });
      setInventoryOpen(false);
      setNotification({ severity: 'success', message: t('inventory.success') });

      try {
        await loadArticle();
      } catch {
        setNotification({ severity: 'error', message: t('inventory.refreshError') });
      }
    } catch (caughtError) {
      const message = translateApiError(caughtError, t);
      setInventoryError(message);
      setNotification({ severity: 'error', message });
    } finally {
      setInventorySubmitting(false);
    }
  };

  const openSaleDialog = () => {
    setSaleQuantity('');
    setSaleMode(article?.type === 'Food' && article.allowedSaleModes.length === 1 ? article.allowedSaleModes[0] : '');
    setSaleError(null);
    setSaleOpen(true);
  };

  const submitSale = async () => {
    if (!id || !article) return;
    const quantity = Number(saleQuantity);
    if (!Number.isInteger(quantity) || quantity <= 0) {
      setSaleError(t('sale.validation.quantity'));
      return;
    }
    if (article.type === 'Food' && !saleMode) {
      setSaleError(t('sale.validation.saleMode'));
      return;
    }

    setSaleSubmitting(true);
    setSaleError(null);
    try {
      await recordSale(id, {
        quantity,
        saleMode: article.type === 'Food' ? saleMode || null : null,
      });
      setSaleOpen(false);
      setNotification({ severity: 'success', message: t('sale.success') });

      try {
        await loadArticle();
      } catch {
        setNotification({ severity: 'error', message: t('sale.refreshError') });
      }
    } catch (caughtError) {
      const message = translateApiError(caughtError, t);
      setSaleError(message);
      setNotification({ severity: 'error', message });
    } finally {
      setSaleSubmitting(false);
    }
  };

  const openEditDialog = () => {
    if (!article?.isActive) return;
    setEditName(article.name);
    setEditPrice(String(article.priceExcludingTax));
    setEditSaleModes(article.allowedSaleModes);
    setEditError(null);
    setEditOpen(true);
  };

  const toggleEditSaleMode = (mode: SaleMode) => {
    setEditSaleModes((current) => current.includes(mode)
      ? current.filter((item) => item !== mode)
      : [...current, mode]);
  };

  const submitEdit = async () => {
    if (!id || !article) return;
    const price = Number(editPrice);
    if (!editName.trim()) {
      setEditError(t('articleEdit.validation.name'));
      return;
    }
    if (!Number.isFinite(price) || price < 0) {
      setEditError(t('articleEdit.validation.price'));
      return;
    }
    if (article.type === 'Food' && editSaleModes.length === 0) {
      setEditError(t('articleEdit.validation.saleMode'));
      return;
    }

    setEditSubmitting(true);
    setEditError(null);
    try {
      await updateArticle(id, {
        name: editName.trim(),
        priceExcludingTax: price,
        allowedSaleModes: article.type === 'Food' ? editSaleModes : null,
      });
      setEditOpen(false);
      setNotification({ severity: 'success', message: t('articleEdit.success') });
      try {
        await loadArticle();
      } catch {
        setNotification({ severity: 'error', message: t('articleEdit.refreshError') });
      }
    } catch (caughtError) {
      const message = translateApiError(caughtError, t);
      setEditError(message);
      setNotification({ severity: 'error', message });
    } finally {
      setEditSubmitting(false);
    }
  };

  if (!article && !error) {
    return <DashboardContent><Box sx={{ display: 'flex', justifyContent: 'center', py: 10 }}><CircularProgress /></Box></DashboardContent>;
  }

  if (error || !article) {
    return <DashboardContent><Alert severity="error">{error ?? t('articleDetails.notFound')}</Alert></DashboardContent>;
  }

  const renderPrices = (selector: (price: ArticlePrice) => number, formatter: (value: number) => string) => (
    <Box sx={{ display: 'grid', gap: 0.5 }}>
      {article.prices.map((price) => (
        <Typography key={price.saleMode ?? 'default'} variant="body2">
          {price.saleMode ? `${t(`saleModes.${price.saleMode === 'TakeAway' ? 'takeAway' : 'onSite'}`)}: ` : ''}
          {formatter(selector(price))}
        </Typography>
      ))}
    </Box>
  );

  const cards = [
    { label: t('articleDetails.totalStock'), value: String(article.totalStock) },
    { label: t('articleDetails.sellableStock'), value: String(article.sellableStock) },
    { label: t('articleDetails.nonSellableStock'), value: String(article.nonSellableStock) },
    { label: t('articles.priceExcludingTax'), value: formatMoney(article.priceExcludingTax, locale) },
  ];
  const availableInventoryBuckets = inventorySearchOptions.filter(
    (bucket) => !inventoryExistingLines.some((line) => line.bucket.id === bucket.id)
  );
  const inventorySystemTotal = inventoryExistingLines.reduce(
    (total, line) => total + line.bucket.physicalQuantity,
    0
  );
  const inventoryCountedTotal = [
    ...inventoryExistingLines.map((line) => Number(line.countedQuantity)),
    ...inventoryNewLines.map((line) => Number(line.countedQuantity)),
  ].reduce((total, quantity) => total + (Number.isFinite(quantity) ? quantity : 0), 0);

  return (
    <DashboardContent>
      <Snackbar open={notification !== null} autoHideDuration={4000} onClose={() => setNotification(null)}>
        <Alert severity={notification?.severity ?? 'success'} onClose={() => setNotification(null)}>{notification?.message}</Alert>
      </Snackbar>
      <Box sx={(theme) => ({ mb: 3, p: { xs: 2.5, md: 3.5 }, display: 'flex', alignItems: 'center', gap: 2, borderRadius: 3, color: 'common.white', background: `linear-gradient(135deg, ${theme.vars.palette.primary.main} 0%, ${theme.vars.palette.secondary.dark} 100%)`, boxShadow: theme.vars.customShadows.primary })}>
        <Box sx={{ flexGrow: 1 }}>
          <Typography variant="h4">{article.name}</Typography>
          <Typography sx={{ color: 'rgba(255,255,255,0.78)' }}>{article.reference}</Typography>
        </Box>
        <Button onClick={openEditDialog} disabled={!article.isActive} sx={{ color: 'common.white', borderColor: 'rgba(255,255,255,0.5)' }} variant="outlined">{t('articleEdit.action')}</Button>
        <Button onClick={() => navigate('/articles')} sx={{ color: 'common.white', borderColor: 'rgba(255,255,255,0.5)' }} variant="outlined">{t('articleDetails.backToList')}</Button>
      </Box>

      {!article.isActive && <Alert severity="warning" sx={{ mb: 3 }}>{t('articleDetails.inactiveWarning')}</Alert>}

      <Card sx={{ p: 3, mb: 3, borderLeft: 4, borderLeftColor: article.type === 'Food' ? 'success.main' : 'secondary.main' }}>
        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'repeat(3, 1fr)' }, gap: 2 }}>
          <Info label={t('articles.reference')} value={article.reference} />
          <Info label={t('articles.name')} value={article.name} />
          <Info label={t('articles.type')} value={t(`articleTypes.${article.type === 'Food' ? 'food' : 'nonFood'}`)} />
          <Info label={t('articles.status')} value={t(`articleStatuses.${article.isActive ? 'active' : 'inactive'}`)} />
          <Info label={t('articles.priceExcludingTax')} value={formatMoney(article.priceExcludingTax, locale)} />
          <Box><Typography variant="caption" color="text.secondary">{t('articles.priceIncludingTax')}</Typography>{renderPrices((price) => price.priceIncludingTax, (value) => formatMoney(value, locale))}</Box>
          <Box><Typography variant="caption" color="text.secondary">{t('articles.vat')}</Typography>{renderPrices((price) => price.vatRate, (value) => formatVat(value, locale))}</Box>
          {article.type === 'Food' && <Info label={t('articleCreate.allowedSaleModes')} value={article.allowedSaleModes.map((mode) => t(`saleModes.${mode === 'TakeAway' ? 'takeAway' : 'onSite'}`)).join(', ')} />}
        </Box>
      </Card>

      <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: 'repeat(2, 1fr)', lg: 'repeat(6, 1fr)' }, gap: 2, mb: 3 }}>
        {cards.map((card, index) => <Card key={card.label} sx={{ p: 2, borderTop: 3, borderTopColor: ['primary.main', 'success.main', 'warning.main', 'secondary.main'][index] }}><Typography variant="caption" color="text.secondary">{card.label}</Typography><Typography variant="h6" sx={{ color: index === 1 ? 'success.dark' : 'text.primary' }}>{card.value}</Typography></Card>)}
        <Card sx={{ p: 2, borderTop: 3, borderTopColor: 'info.main' }}><Typography variant="caption" color="text.secondary">{t('articles.priceIncludingTax')}</Typography>{renderPrices((price) => price.priceIncludingTax, (value) => formatMoney(value, locale))}</Card>
        <Card sx={{ p: 2, borderTop: 3, borderTopColor: 'warning.main' }}><Typography variant="caption" color="text.secondary">{t('articles.vat')}</Typography>{renderPrices((price) => price.vatRate, (value) => formatVat(value, locale))}</Card>
      </Box>

      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2, mb: 3 }}>
        <Button variant="contained" disabled={!article.isActive} onClick={openSupplyDialog}>{t('articleDetails.supply')}</Button>
        <Button variant="contained" disabled={!article.isActive} onClick={openSaleDialog}>{t('articleDetails.sale')}</Button>
        <Button variant="contained" disabled={!article.isActive} onClick={openInventoryDialog}>{t('articleDetails.inventory')}</Button>
      </Box>

      <Card sx={{ mb: 3, overflow: 'hidden' }}>
        <Typography variant="h6" sx={{ p: 3, pb: 1 }}>{t('articleDetails.stockBuckets')}</Typography>
        <TableContainer><Table>
          <TableHead><TableRow>
            <TableCell>{t('articleDetails.bucketReference')}</TableCell>
            <TableCell>{article.type === 'Food' ? t('articleDetails.expirationDate') : t('articleDetails.packaging')}</TableCell>
            <TableCell>{t('articleDetails.physicalQuantity')}</TableCell>
            <TableCell>{t('articleDetails.sellableQuantity')}</TableCell>
            <TableCell>{t('articleDetails.status')}</TableCell>
          </TableRow></TableHead>
          <TableBody>
            {article.buckets.length === 0 ? (
              <TableRow><TableCell colSpan={5} align="center" sx={{ py: 6 }}>{t('articleDetails.noBuckets')}</TableCell></TableRow>
            ) : article.buckets.map((bucket) => (
              <TableRow key={bucket.id}>
                <TableCell sx={{ fontWeight: 600 }}>{bucket.reference}</TableCell>
                <TableCell>
                  {bucket.type === 'Food' && bucket.expirationDate ? formatDateOnly(bucket.expirationDate, locale) : null}
                  {bucket.type === 'NonFood' && bucket.packagingLevel ? t(`packaging.${bucket.packagingLevel}`) : null}
                </TableCell>
                <TableCell>{bucket.physicalQuantity}</TableCell>
                <TableCell>{bucket.sellableQuantity}</TableCell>
                <TableCell>
                  <Chip
                    size="small"
                    label={t(`bucketStatuses.${bucket.status}`)}
                    color={bucket.status === 'Sellable' ? 'success' : bucket.status === 'Empty' ? 'default' : 'warning'}
                    variant={bucket.status === 'Empty' ? 'outlined' : 'filled'}
                  />
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table></TableContainer>
      </Card>

      <Card sx={{ mb: 3, overflow: 'hidden' }}>
        <Typography variant="h6" sx={{ p: 3, pb: 1 }}>{t('articleDetails.movements')}</Typography>
        <TableContainer><Table>
          <TableHead><TableRow>
            <TableCell width={48} />
            <TableCell>{t('articleDetails.date')}</TableCell>
            <TableCell>{t('articleDetails.movementType')}</TableCell>
            <TableCell>{t('articleDetails.quantityDelta')}</TableCell>
            <TableCell>{t('articleDetails.impactedBuckets')}</TableCell>
          </TableRow></TableHead>
          <TableBody>
            {article.movements.length === 0
              ? <TableRow><TableCell colSpan={5} align="center" sx={{ py: 6 }}>{t('articleDetails.noMovements')}</TableCell></TableRow>
              : article.movements.map((movement) => <MovementRows key={movement.id} movement={movement} locale={locale} t={t} />)}
          </TableBody>
        </Table></TableContainer>
      </Card>

      <Dialog open={supplyOpen} onClose={() => !supplySubmitting && setSupplyOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>{t('supply.title')}</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'grid', gap: 3, pt: 1 }}>
            {supplyError && <Alert severity="error">{supplyError}</Alert>}
            <TextField
              required
              label={t('supply.reference')}
              value={supplyReferenceDigits}
              inputProps={{ inputMode: 'numeric', maxLength: 13, pattern: '[0-9]*' }}
              InputProps={{ startAdornment: <InputAdornment position="start">ref-lot-</InputAdornment> }}
              onChange={(event) => setSupplyReferenceDigits(sanitizeReferenceDigits(event.target.value))}
            />
            <TextField
              required
              type="number"
              label={t('supply.quantity')}
              value={supplyQuantity}
              inputProps={{ min: 1, step: 1 }}
              onChange={(event) => setSupplyQuantity(event.target.value)}
            />
            {article.type === 'Food' ? (
              <TextField
                required
                type="date"
                label={t('supply.expirationDate')}
                value={supplyExpirationDate}
                InputLabelProps={{ shrink: true }}
                onChange={(event) => setSupplyExpirationDate(event.target.value)}
              />
            ) : (
              <FormControl required>
                <InputLabel id="supply-packaging-label">{t('supply.packaging')}</InputLabel>
                <Select
                  labelId="supply-packaging-label"
                  label={t('supply.packaging')}
                  value={supplyPackaging}
                  onChange={(event) => setSupplyPackaging(event.target.value as typeof supplyPackaging)}
                >
                  <MenuItem value="New">{t('packaging.New')}</MenuItem>
                  <MenuItem value="Refurbished">{t('packaging.Refurbished')}</MenuItem>
                  <MenuItem value="Unsellable">{t('packaging.Unsellable')}</MenuItem>
                </Select>
              </FormControl>
            )}
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setSupplyOpen(false)} disabled={supplySubmitting}>{t('common.cancel')}</Button>
          <Button variant="contained" onClick={submitSupply} disabled={supplySubmitting}>
            {supplySubmitting ? t('common.loading') : t('supply.submit')}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={editOpen} onClose={() => !editSubmitting && setEditOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>{t('articleEdit.title')}</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'grid', gap: 3, pt: 1 }}>
            {editError && <Alert severity="error">{editError}</Alert>}
            <TextField required label={t('articles.name')} value={editName} onChange={(event) => setEditName(event.target.value)} />
            <TextField required type="number" label={t('articles.priceExcludingTax')} value={editPrice} inputProps={{ min: 0, step: 0.01 }} onChange={(event) => setEditPrice(event.target.value)} />
            {article.type === 'Food' && (
              <Box>
                <Typography variant="subtitle2">{t('articleCreate.allowedSaleModes')}</Typography>
                <FormControlLabel control={<Checkbox checked={editSaleModes.includes('TakeAway')} onChange={() => toggleEditSaleMode('TakeAway')} />} label={t('saleModes.takeAway')} />
                <FormControlLabel control={<Checkbox checked={editSaleModes.includes('OnSite')} onChange={() => toggleEditSaleMode('OnSite')} />} label={t('saleModes.onSite')} />
              </Box>
            )}
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setEditOpen(false)} disabled={editSubmitting}>{t('common.cancel')}</Button>
          <Button variant="contained" onClick={submitEdit} disabled={editSubmitting}>{editSubmitting ? t('common.loading') : t('common.save')}</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={inventoryOpen} onClose={() => !inventorySubmitting && setInventoryOpen(false)} fullWidth maxWidth="lg">
        <DialogTitle>{t('inventory.title')}</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'grid', gap: 3, pt: 1 }}>
            {inventoryError && <Alert severity="error">{inventoryError}</Alert>}
            {article.buckets.length === 0 && <Alert severity="info">{t('inventory.noExistingBuckets')}</Alert>}
            <Box sx={{ display: 'flex', gap: 2, alignItems: 'center' }}>
              <Autocomplete
                fullWidth
                options={availableInventoryBuckets}
                value={inventoryBucketToAdd}
                inputValue={inventorySearchDigits}
                loading={inventorySearchLoading}
                filterOptions={(options) => options}
                getOptionLabel={(option) => option.reference.replace('ref-lot-', '')}
                isOptionEqualToValue={(option, value) => option.id === value.id}
                onChange={(_, value) => {
                  setInventoryBucketToAdd(value);
                  if (value) setInventorySearchDigits(value.reference.replace('ref-lot-', ''));
                }}
                onInputChange={(_, value, reason) => {
                  if (reason === 'input') {
                    setInventorySearchDigits(sanitizeReferenceDigits(value));
                    setInventoryBucketToAdd(null);
                  }
                }}
                noOptionsText={inventorySearchDigits.length < 9 ? t('inventory.searchMinimum') : t('inventory.noSearchResults')}
                loadingText={t('common.loading')}
                renderInput={(params) => (
                  <TextField
                    {...params}
                    label={t('inventory.searchBucket')}
                    inputProps={{ ...params.inputProps, inputMode: 'numeric', maxLength: 13 }}
                    InputProps={{
                      ...params.InputProps,
                      startAdornment: (
                        <>
                          <InputAdornment position="start">ref-lot-</InputAdornment>
                          {params.InputProps.startAdornment}
                        </>
                      ),
                    }}
                  />
                )}
              />
              <Button variant="outlined" onClick={addExistingInventoryBucket} disabled={!inventoryBucketToAdd}>
                {t('inventory.add')}
              </Button>
            </Box>

            <Box>
              <Button variant="outlined" startIcon={<Iconify icon="mingcute:add-line" />} onClick={addNewInventoryBucket}>
                {t('inventory.addNewBucket')}
              </Button>
            </Box>

            <TableContainer><Table size="small">
              <TableHead><TableRow>
                <TableCell>{t('articleDetails.bucketReference')}</TableCell>
                <TableCell>{article.type === 'Food' ? t('articleDetails.expirationDate') : t('articleDetails.packaging')}</TableCell>
                <TableCell>{t('inventory.systemQuantity')}</TableCell>
                <TableCell>{t('inventory.countedQuantity')}</TableCell>
                <TableCell>{t('inventory.difference')}</TableCell>
                <TableCell>{t('articleDetails.status')}</TableCell>
                <TableCell>{t('articles.actions')}</TableCell>
              </TableRow></TableHead>
              <TableBody>
                {inventoryExistingLines.map((line) => {
                  const counted = Number(line.countedQuantity);
                  const difference = Number.isFinite(counted) ? counted - line.bucket.physicalQuantity : 0;
                  return (
                    <TableRow key={line.bucket.id}>
                      <TableCell>{line.bucket.reference}</TableCell>
                      <TableCell>
                        {line.bucket.expirationDate ? formatDateOnly(line.bucket.expirationDate, locale) : null}
                        {line.bucket.packagingLevel ? t(`packaging.${line.bucket.packagingLevel}`) : null}
                      </TableCell>
                      <TableCell>{line.bucket.physicalQuantity}</TableCell>
                      <TableCell><TextField size="small" type="number" value={line.countedQuantity} inputProps={{ min: 0, step: 1 }} onChange={(event) => setInventoryExistingLines((current) => current.map((item) => item.bucket.id === line.bucket.id ? { ...item, countedQuantity: event.target.value } : item))} /></TableCell>
                      <TableCell sx={{ color: difference === 0 ? 'text.secondary' : difference > 0 ? 'success.main' : 'error.main', fontWeight: 700 }}>{difference > 0 ? `+${difference}` : difference}</TableCell>
                      <TableCell><Chip size="small" label={t(`bucketStatuses.${line.bucket.status}`)} /></TableCell>
                      <TableCell><IconButton aria-label={t('inventory.remove')} onClick={() => setInventoryExistingLines((current) => current.filter((item) => item.bucket.id !== line.bucket.id))}><Iconify icon="solar:trash-bin-trash-bold" /></IconButton></TableCell>
                    </TableRow>
                  );
                })}
                {inventoryNewLines.map((line) => {
                  const counted = Number(line.countedQuantity);
                  const difference = Number.isFinite(counted) ? counted : 0;
                  return (
                    <TableRow key={line.key}>
                      <TableCell><TextField size="small" required value={line.referenceDigits} inputProps={{ inputMode: 'numeric', maxLength: 13, pattern: '[0-9]*' }} InputProps={{ startAdornment: <InputAdornment position="start">ref-lot-</InputAdornment> }} onChange={(event) => setInventoryNewLines((current) => current.map((item) => item.key === line.key ? { ...item, referenceDigits: sanitizeReferenceDigits(event.target.value) } : item))} /></TableCell>
                      <TableCell>
                        {article.type === 'Food' ? <TextField size="small" required type="date" value={line.expirationDate} onChange={(event) => setInventoryNewLines((current) => current.map((item) => item.key === line.key ? { ...item, expirationDate: event.target.value } : item))} /> : <Select size="small" value={line.packagingLevel} onChange={(event) => setInventoryNewLines((current) => current.map((item) => item.key === line.key ? { ...item, packagingLevel: event.target.value as InventoryNewLine['packagingLevel'] } : item))}><MenuItem value="New">{t('packaging.New')}</MenuItem><MenuItem value="Refurbished">{t('packaging.Refurbished')}</MenuItem><MenuItem value="Unsellable">{t('packaging.Unsellable')}</MenuItem></Select>}
                      </TableCell>
                      <TableCell>0</TableCell>
                      <TableCell><TextField size="small" required type="number" value={line.countedQuantity} inputProps={{ min: 1, step: 1 }} onChange={(event) => setInventoryNewLines((current) => current.map((item) => item.key === line.key ? { ...item, countedQuantity: event.target.value } : item))} /></TableCell>
                      <TableCell sx={{ color: 'success.main', fontWeight: 700 }}>{difference > 0 ? `+${difference}` : difference}</TableCell>
                      <TableCell><Chip size="small" color="info" label={t('inventory.newBucket')} /></TableCell>
                      <TableCell><IconButton aria-label={t('inventory.remove')} onClick={() => setInventoryNewLines((current) => current.filter((item) => item.key !== line.key))}><Iconify icon="solar:trash-bin-trash-bold" /></IconButton></TableCell>
                    </TableRow>
                  );
                })}
                {inventoryExistingLines.length === 0 && inventoryNewLines.length === 0 && <TableRow><TableCell colSpan={7} align="center" sx={{ py: 4 }}>{t('inventory.noSelectedBuckets')}</TableCell></TableRow>}
              </TableBody>
            </Table></TableContainer>

            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 3 }}>
              <Info label={t('inventory.systemTotal')} value={String(inventorySystemTotal)} />
              <Info label={t('inventory.countedTotal')} value={String(inventoryCountedTotal)} />
              <Info label={t('inventory.totalDifference')} value={String(inventoryCountedTotal - inventorySystemTotal)} />
            </Box>
            <TextField multiline minRows={2} label={t('inventory.comment')} value={inventoryComment} onChange={(event) => setInventoryComment(event.target.value)} />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setInventoryOpen(false)} disabled={inventorySubmitting}>{t('common.cancel')}</Button>
          <Button variant="contained" onClick={submitInventory} disabled={inventorySubmitting}>
            {inventorySubmitting ? t('common.loading') : t('inventory.submit')}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={saleOpen} onClose={() => !saleSubmitting && setSaleOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>{t('sale.title')}</DialogTitle>
        <DialogContent>
          <Box sx={{ display: 'grid', gap: 3, pt: 1 }}>
            {saleError && <Alert severity="error">{saleError}</Alert>}
            <TextField
              required
              type="number"
              label={t('sale.quantity')}
              value={saleQuantity}
              inputProps={{ min: 1, step: 1 }}
              onChange={(event) => setSaleQuantity(event.target.value)}
            />
            {article.type === 'Food' && (
              <FormControl required>
                <InputLabel id="sale-mode-label">{t('sale.saleMode')}</InputLabel>
                <Select
                  labelId="sale-mode-label"
                  label={t('sale.saleMode')}
                  value={saleMode}
                  onChange={(event) => setSaleMode(event.target.value as SaleMode)}
                >
                  {article.allowedSaleModes.map((mode) => (
                    <MenuItem key={mode} value={mode}>
                      {t(`saleModes.${mode === 'TakeAway' ? 'takeAway' : 'onSite'}`)}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            )}
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setSaleOpen(false)} disabled={saleSubmitting}>{t('common.cancel')}</Button>
          <Button variant="contained" onClick={submitSale} disabled={saleSubmitting}>
            {saleSubmitting ? t('common.loading') : t('sale.submit')}
          </Button>
        </DialogActions>
      </Dialog>
    </DashboardContent>
  );
}

function Info({ label, value }: { label: string; value: string }) {
  return <Box><Typography variant="caption" color="text.secondary">{label}</Typography><Typography>{value}</Typography></Box>;
}

function MovementRows({ movement, locale, t }: { movement: StockMovement; locale: string; t: (key: string) => string }) {
  const [open, setOpen] = useState(false);
  const canExpand = movement.type !== 'Supply';

  return (
    <Fragment>
      <TableRow hover>
        <TableCell>
          {canExpand && (
            <IconButton size="small" onClick={() => setOpen((current) => !current)} aria-label={t('articleDetails.toggleMovementLines')}>
              <Iconify icon={open ? 'eva:arrow-ios-upward-fill' : 'eva:arrow-ios-downward-fill'} />
            </IconButton>
          )}
        </TableCell>
        <TableCell>{new Intl.DateTimeFormat(locale, { dateStyle: 'short', timeStyle: 'short' }).format(new Date(movement.createdAt))}</TableCell>
        <TableCell>{t(`movementTypes.${movement.type}`)}</TableCell>
        <TableCell sx={{ fontWeight: 700, color: movement.quantityDelta >= 0 ? 'success.main' : 'error.main' }}>
          {movement.quantityDelta > 0 ? `+${movement.quantityDelta}` : movement.quantityDelta}
        </TableCell>
        <TableCell>{movement.lines.length}</TableCell>
      </TableRow>
      {canExpand && <TableRow>
        <TableCell sx={{ p: 0 }} colSpan={5}>
          <Collapse in={open} timeout="auto" unmountOnExit>
            <Box sx={{ p: 2, bgcolor: 'background.neutral' }}>
              {movement.type === 'Sale' && (
                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 3, mb: 2 }}>
                  {movement.saleMode && (
                    <Info
                      label={t('sale.saleMode')}
                      value={t(`saleModes.${movement.saleMode === 'TakeAway' ? 'takeAway' : 'onSite'}`)}
                    />
                  )}
                  <Info label={t('sale.soldQuantity')} value={String(movement.soldQuantity ?? 0)} />
                  <Info label={t('sale.unitPriceExcludingTax')} value={formatMoney(movement.unitPriceExcludingTax ?? 0, locale)} />
                  <Info label={t('sale.unitPriceIncludingTax')} value={formatMoney(movement.unitPriceIncludingTax ?? 0, locale)} />
                  <Info label={t('sale.vatRate')} value={formatVat(movement.vatRate ?? 0, locale)} />
                  <Info label={t('sale.totalExcludingTax')} value={formatMoney(movement.totalExcludingTax ?? 0, locale)} />
                  <Info label={t('sale.totalIncludingTax')} value={formatMoney(movement.totalIncludingTax ?? 0, locale)} />
                </Box>
              )}
              {movement.type === 'Inventory' && movement.comment && (
                <Box sx={{ mb: 2 }}>
                  <Info label={t('inventory.comment')} value={movement.comment} />
                </Box>
              )}
              <Table size="small">
                <TableHead><TableRow>
                  <TableCell>{t('articleDetails.bucket')}</TableCell>
                  <TableCell>{t('articleDetails.quantityDelta')}</TableCell>
                  <TableCell>{t('articleDetails.quantityBefore')}</TableCell>
                  <TableCell>{t('articleDetails.quantityAfter')}</TableCell>
                </TableRow></TableHead>
                <TableBody>{movement.lines.map((line) => (
                  <TableRow key={line.id}>
                    <TableCell>
                      <Typography variant="body2" sx={{ fontWeight: 600 }}>{line.stockBucketReference}</Typography>
                      {line.bucketType === 'Food' && line.expirationDate ? `${t('articleDetails.expirationDate')} ${formatDateOnly(line.expirationDate, locale)}` : null}
                      {line.bucketType === 'NonFood' && line.packagingLevel ? `${t('articleDetails.packaging')} ${t(`packaging.${line.packagingLevel}`)}` : null}
                    </TableCell>
                    <TableCell>{line.quantityDelta > 0 ? `+${line.quantityDelta}` : line.quantityDelta}</TableCell>
                    <TableCell>{line.quantityBefore ?? '—'}</TableCell>
                    <TableCell>{line.quantityAfter ?? '—'}</TableCell>
                  </TableRow>
                ))}</TableBody>
              </Table>
            </Box>
          </Collapse>
        </TableCell>
      </TableRow>}
    </Fragment>
  );
}
