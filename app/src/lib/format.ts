const currencyFormatter = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' });
const dateTimeFormatter = new Intl.DateTimeFormat('en-US', { dateStyle: 'medium', timeStyle: 'short' });
const unitPriceFormatter = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 2, maximumFractionDigits: 4 });
const weightFormatter = new Intl.NumberFormat('en-US', { maximumFractionDigits: 4 });

export function formatCurrency(value: number) {
  return currencyFormatter.format(value);
}

export function formatDateTime(value: string) {
  return dateTimeFormatter.format(new Date(value));
}

export function formatUnitPrice(value: number) {
  return unitPriceFormatter.format(value);
}

export function formatWeight(value: number) {
  return `${weightFormatter.format(value)} lbs`;
}