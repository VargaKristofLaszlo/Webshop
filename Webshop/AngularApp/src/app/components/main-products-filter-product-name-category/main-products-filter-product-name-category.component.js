"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.MainProductsFilterProductNameCategoryComponent = void 0;
var core_1 = require("@angular/core");
var app_component_1 = require("../../app.component");
var file_1 = require("../../classes/file");
var MainProductsFilterProductNameCategoryComponent = /** @class */ (function (_super) {
    __extends(MainProductsFilterProductNameCategoryComponent, _super);
    function MainProductsFilterProductNameCategoryComponent(categoryService, productService, reviewService, router, toastr) {
        var _this = _super.call(this) || this;
        _this.categoryService = categoryService;
        _this.productService = productService;
        _this.reviewService = reviewService;
        _this.router = router;
        _this.toastr = toastr;
        _this.ProductImageNameList = [];
        return _this;
    }
    MainProductsFilterProductNameCategoryComponent.prototype.ngOnInit = function () {
        this.refreshCategoryList();
        var categoryId_json = localStorage.getItem('categoryId');
        var productName_json = localStorage.getItem('productName');
        if (categoryId_json == null)
            this.selectedOption_category = JSON.parse(JSON.stringify(-1));
        else {
            this.refreshProductList(JSON.parse(categoryId_json), JSON.parse(productName_json));
            this.inputFieldName = JSON.parse(productName_json);
            this.selectedOption_category = JSON.parse(categoryId_json);
        }
        this.isLoggedIn = _super.prototype.tokenCheck.call(this);
    };
    MainProductsFilterProductNameCategoryComponent.prototype.refreshCategoryList = function () {
        var _this = this;
        this.categoryService.getAll().subscribe(function (data) {
            _this.CategoryList = data;
        });
    };
    MainProductsFilterProductNameCategoryComponent.prototype.refreshReviewList = function (product) {
        var test = new file_1.Global_Functions();
        test.refreshReviewList(product, this.reviewService, this.toastr);
    };
    MainProductsFilterProductNameCategoryComponent.prototype.refreshProductList = function (categoryId, productName) {
        var _this = this;
        this.productService.GetByCategoryIdAndProductName(categoryId, productName).subscribe(function (data) {
            _this.ProductList = data;
            for (var _i = 0, _a = _this.ProductList; _i < _a.length; _i++) {
                var product = _a[_i];
                _this.ProductImageNameList.push(_this.imageRoute + product.imageName);
                _this.refreshReviewList(product);
            }
            ;
            console.log(data);
        });
    };
    MainProductsFilterProductNameCategoryComponent.prototype.removeFilterFromProducts = function () {
        this.router.navigateByUrl("/techonomy/products/category/nofilter");
    };
    MainProductsFilterProductNameCategoryComponent.prototype.filterClicked = function () {
        //Remove the previous filter form the products
        this.removeFilterFromProducts();
        var teszt = new file_1.Global_Functions();
        switch (teszt.filterClicked(this.inputFieldName, this.selectedOption_category)) {
            case "filterByName": {
                this.filterByName(this.inputFieldName);
                break;
            }
            case "filterByCategory": {
                this.filterByCategory(JSON.parse(this.selectedOption_category));
                break;
            }
            case "filterByNameAndCategory": {
                this.filterByNameAndCategory(this.inputFieldName, JSON.parse(this.selectedOption_category));
                break;
            }
            default: {
                this.removeFilterFromProducts();
            }
        }
    };
    MainProductsFilterProductNameCategoryComponent.prototype.filterByName = function (productName) {
        localStorage.setItem('productName', JSON.stringify(productName));
        this.router.navigateByUrl('techonomy/products/category/productnameFilter/' + productName);
    };
    MainProductsFilterProductNameCategoryComponent.prototype.filterByCategory = function (categoryId) {
        localStorage.setItem('categoryId', JSON.stringify(categoryId));
        this.router.navigateByUrl('techonomy/products/category/categoryFilter/' + categoryId);
    };
    MainProductsFilterProductNameCategoryComponent.prototype.filterByNameAndCategory = function (productName, categoryId) {
        localStorage.setItem('productName', JSON.stringify(productName));
        localStorage.setItem('categoryId', JSON.stringify(categoryId));
        this.router.navigateByUrl('techonomy/products/category/mixedFilter/categoryId/' + categoryId + "/productName/" + productName);
        this.ProductImageNameList = [];
        this.ngOnInit();
    };
    MainProductsFilterProductNameCategoryComponent.prototype.productPictureClicked = function (product) {
        localStorage.setItem('product', JSON.stringify(product));
        this.router.navigateByUrl('techonomy/products/' + product.productID);
    };
    //Categóriára való szűrés navbar-ból
    MainProductsFilterProductNameCategoryComponent.prototype.categorySelector = function (categoryId) {
        localStorage.setItem('categoryId', JSON.stringify(categoryId));
        this.router.navigateByUrl('techonomy/products/category/categoryFilter/' + categoryId);
        this.ProductImageNameList = [];
        this.ngOnInit();
    };
    //User kiléptetés && bejelenetkezés ellenőrzés
    MainProductsFilterProductNameCategoryComponent.prototype.checkLogin = function () {
        _super.prototype.checkLogin.call(this, this.isLoggedIn, this.router);
    };
    MainProductsFilterProductNameCategoryComponent.prototype.onLogout = function () {
        _super.prototype.onLogout.call(this, this.router);
    };
    __decorate([
        core_1.Input()
    ], MainProductsFilterProductNameCategoryComponent.prototype, "selectedOption_category", void 0);
    MainProductsFilterProductNameCategoryComponent = __decorate([
        core_1.Component({
            selector: 'app-main-products-filter-product-name-category',
            templateUrl: './main-products-filter-product-name-category.component.html',
            styleUrls: ['./main-products-filter-product-name-category.component.css']
        })
    ], MainProductsFilterProductNameCategoryComponent);
    return MainProductsFilterProductNameCategoryComponent;
}(app_component_1.AppComponent));
exports.MainProductsFilterProductNameCategoryComponent = MainProductsFilterProductNameCategoryComponent;
//# sourceMappingURL=main-products-filter-product-name-category.component.js.map