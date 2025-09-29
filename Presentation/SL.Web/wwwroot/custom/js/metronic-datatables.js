"use strict";

/**
 * MetronicDataTables
 * Projedeki tüm sunucu taraflı DataTables listelerini yönetmek için kullanılan jenerik bir yardımcı.
 * Yapılandırma tabanlıdır ve kod tekrarını önler.
 */
var MetronicDataTables = function () {
    // Private değişkenler, her init çağrısında yeniden ayarlanır
    let table;
    let datatable;
    let config;

    // DataTables'ı başlatan ana fonksiyon
    const _initDatatable = function () {
        const antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

        datatable = $(table).DataTable({
            // Varsayılan ayarlar
            info: false,
            order: [],
            processing: true,
            serverSide: true,
            
            // DÜZELTME: Responsive özelliğini konfigürasyondan al.
            // Eğer belirtilmemişse, varsayılan olarak 'false' (kapalı) olsun.
            responsive: config.responsive || false,
            
            ajax: {
                url: $(table).data('get-url'),
                type: 'POST',
                headers: { "RequestVerificationToken": antiForgeryToken },
                data: function (d) {
                    if (config.ajaxDataFunction && typeof config.ajaxDataFunction === 'function') {
                        config.ajaxDataFunction(d);
                    }
                }
            },
            columns: config.columns,
            columnDefs: config.columnDefs || []
        });

        datatable.on('draw', function () {
            _initDeleteOrToggleActions();
        });
    };

    // Genel arama kutusunu yöneten fonksiyon
    const _handleSearch = function () {
        if (!config.search || !config.search.input) return;

        const searchInput = document.querySelector(config.search.input);
        if (!searchInput) return;

        let timer;
        searchInput.addEventListener('keyup', e => {
            clearTimeout(timer);
            timer = setTimeout(function () {
                datatable.search(e.target.value).draw();
            }, 300);
        });
    };
    
    // Özel filtre menüsündeki "Uygula" ve "Sıfırla" butonlarını yöneten fonksiyon
    const _handleFilter = function() {
        if (!config.filter) return;
        
        const applyButton = document.querySelector(config.filter.applyButton);
        const resetButton = document.querySelector(config.filter.resetButton);

        if (applyButton) {
            applyButton.addEventListener('click', function () {
                const menu = this.closest('[data-kt-menu="true"]');
                if (menu) MenuComponent.getInstance(menu).hide();
                datatable.draw();
            });
        }

        if (resetButton) {
            resetButton.addEventListener('click', function () {
                const form = this.closest('[data-kt-menu="true"]');
                const select2s = form.querySelectorAll('select');
                select2s.forEach(select => {
                    $(select).val(null).trigger('change');
                });
                const menu = this.closest('[data-kt-menu="true"]');
                if (menu) MenuComponent.getInstance(menu).hide();
                datatable.draw();
            });
        }
    }

    // Satırlardaki Silme veya Durum Değiştirme gibi aksiyonları yöneten fonksiyon
    const _initDeleteOrToggleActions = function () {
        if (!config.action || !config.action.selector) return;

        const actionButtons = table.querySelectorAll(config.action.selector);
        actionButtons.forEach(button => {
            if (button.getAttribute('listener') === 'true') {
                return;
            }
            button.setAttribute('listener', 'true');
            
            button.addEventListener('click', e => {
                e.preventDefault();
                const link = e.target.closest('a');
                
                let confirmationText;
                if (config.action.confirmationTextGenerator && typeof config.action.confirmationTextGenerator === 'function') {
                    confirmationText = config.action.confirmationTextGenerator(link);
                } else {
                    const displayName = link.getAttribute('data-display-name');
                    confirmationText = config.action.confirmationText.replace('{displayName}', displayName);
                }

                Swal.fire({
                    text: confirmationText,
                    icon: "warning",
                    showCancelButton: true,
                    buttonsStyling: false,
                    confirmButtonText: config.action.confirmButtonText || "Evet, devam et!",
                    cancelButtonText: "Hayır, vazgeç",
                    customClass: {
                        confirmButton: `btn fw-bold ${config.action.confirmButtonClass || 'btn-danger'}`,
                        cancelButton: "btn fw-bold btn-active-light-primary"
                    }
                }).then(result => {
                    if (result.value) {
                        const recordId = link.getAttribute('data-id');
                        $.ajax({
                            url: config.action.url,
                            type: "POST",
                            data: { id: recordId },
                            headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                            success: response => {
                                if (response.success) {
                                    Swal.fire("Başarılı!", response.message, "success").then(() => datatable.draw());
                                } else {
                                    Swal.fire("Hata!", response.message, "error");
                                }
                            },
                            error: () => {
                                Swal.fire("Hata!", "İşlem sırasında bir sunucu hatası oluştu.", "error");
                            }
                        });
                    }
                });
            });
        });
    };

    // Dışarıya açılan tek public metot
    return {
        init: function (userConfig) {
            config = userConfig;
            table = document.querySelector(config.table);

            if (!table) {
                console.error(`DataTables Error: Tablo bulunamadı -> ${config.table}`);
                return;
            }

            _initDatatable();
            _handleSearch();
            _initDeleteOrToggleActions();
            _handleFilter();
        }
    };
}();