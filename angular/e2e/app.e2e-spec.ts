import { PointOfSalesTemplatePage } from './app.po';

describe('PointOfSales App', function() {
  let page: PointOfSalesTemplatePage;

  beforeEach(() => {
    page = new PointOfSalesTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
