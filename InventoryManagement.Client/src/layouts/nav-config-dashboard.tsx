import { SvgColor } from 'src/components/svg-color';

// ----------------------------------------------------------------------

const icon = (name: string) => <SvgColor src={`/assets/icons/navbar/${name}.svg`} />;

export type NavItem = {
  titleKey: 'navigation.dashboard' | 'navigation.articles';
  path: string;
  icon: React.ReactNode;
};

export const navData: NavItem[] = [
  {
    titleKey: 'navigation.dashboard',
    path: '/',
    icon: icon('ic-analytics'),
  },
  {
    titleKey: 'navigation.articles',
    path: '/articles',
    icon: icon('ic-cart'),
  },
];
