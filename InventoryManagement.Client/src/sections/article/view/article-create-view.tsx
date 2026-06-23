import type { FormEvent } from 'react';
import type { SaleMode } from 'src/api';

import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import Alert from '@mui/material/Alert';
import Button from '@mui/material/Button';
import Select from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import Checkbox from '@mui/material/Checkbox';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import InputLabel from '@mui/material/InputLabel';
import FormControl from '@mui/material/FormControl';
import FormControlLabel from '@mui/material/FormControlLabel';

import { DashboardContent } from 'src/layouts/dashboard';
import { useTranslate, translateApiError } from 'src/locales';
import { createFoodArticle, createNonFoodArticle } from 'src/api';

export function ArticleCreateView() {
  const { t } = useTranslate();
  const navigate = useNavigate();
  const [reference, setReference] = useState('');
  const [name, setName] = useState('');
  const [price, setPrice] = useState('');
  const [type, setType] = useState<'Food' | 'NonFood'>('Food');
  const [saleModes, setSaleModes] = useState<SaleMode[]>(['TakeAway']);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const toggleSaleMode = (mode: SaleMode) => {
    setSaleModes((current) =>
      current.includes(mode) ? current.filter((item) => item !== mode) : [...current, mode]
    );
  };

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setError(null);

    if (!/^\d{13}$/.test(reference.trim())) {
      setError(t('articleCreate.validation.reference'));
      return;
    }
    if (!name.trim()) {
      setError(t('articleCreate.validation.name'));
      return;
    }
    const parsedPrice = Number(price);
    if (price === '' || Number.isNaN(parsedPrice) || parsedPrice < 0) {
      setError(t('articleCreate.validation.price'));
      return;
    }
    if (type === 'Food' && saleModes.length === 0) {
      setError(t('articleCreate.validation.saleMode'));
      return;
    }

    setSubmitting(true);
    try {
      const common = { reference: reference.trim(), name: name.trim(), priceExcludingTax: parsedPrice };
      const created = type === 'Food'
        ? await createFoodArticle({ ...common, saleModes })
        : await createNonFoodArticle(common);
      navigate(`/articles/${created.id}`, { state: { articleCreated: true } });
    } catch (caughtError) {
      setError(translateApiError(caughtError, t));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <DashboardContent>
      <Box sx={(theme) => ({ mb: 3, p: 3, borderRadius: 3, color: 'common.white', background: `linear-gradient(135deg, ${theme.vars.palette.primary.main} 0%, ${theme.vars.palette.secondary.main} 100%)`, boxShadow: theme.vars.customShadows.primary })}>
        <Typography variant="h4">{t('articleCreate.title')}</Typography>
        <Typography sx={{ mt: 1, color: 'rgba(255,255,255,0.78)' }}>{t('articles.description')}</Typography>
      </Box>
      <Card component="form" onSubmit={handleSubmit} sx={{ p: { xs: 2.5, md: 4 }, maxWidth: 760, mx: 'auto', width: 1, borderTop: 4, borderTopColor: type === 'Food' ? 'success.main' : 'secondary.main' }}>
        <Box sx={{ display: 'grid', gap: 3 }}>
          {error && <Alert severity="error">{error}</Alert>}
          <TextField
            required
            label={t('articles.reference')}
            value={reference}
            inputProps={{ maxLength: 13, inputMode: 'numeric' }}
            onChange={(event) => setReference(event.target.value)}
          />
          <TextField required label={t('articles.name')} value={name} onChange={(event) => setName(event.target.value)} />
          <TextField
            required
            type="number"
            label={t('articles.priceExcludingTax')}
            value={price}
            inputProps={{ min: 0, step: '0.01' }}
            onChange={(event) => setPrice(event.target.value)}
          />
          <FormControl>
            <InputLabel id="article-type-label">{t('articles.type')}</InputLabel>
            <Select
              labelId="article-type-label"
              label={t('articles.type')}
              value={type}
              onChange={(event) => setType(event.target.value as 'Food' | 'NonFood')}
            >
              <MenuItem value="Food">{t('articleTypes.food')}</MenuItem>
              <MenuItem value="NonFood">{t('articleTypes.nonFood')}</MenuItem>
            </Select>
          </FormControl>
          {type === 'Food' && (
            <Box>
              <Typography variant="subtitle2" sx={{ mb: 1 }}>{t('articleCreate.allowedSaleModes')}</Typography>
              <FormControlLabel
                control={<Checkbox checked={saleModes.includes('TakeAway')} onChange={() => toggleSaleMode('TakeAway')} />}
                label={t('saleModes.takeAway')}
              />
              <FormControlLabel
                control={<Checkbox checked={saleModes.includes('OnSite')} onChange={() => toggleSaleMode('OnSite')} />}
                label={t('saleModes.onSite')}
              />
            </Box>
          )}
          <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 2 }}>
            <Button onClick={() => navigate('/articles')}>{t('common.cancel')}</Button>
            <Button type="submit" variant="contained" color="primary" disabled={submitting} sx={{ px: 4 }}>
              {submitting ? t('common.loading') : t('common.save')}
            </Button>
          </Box>
        </Box>
      </Card>
    </DashboardContent>
  );
}
