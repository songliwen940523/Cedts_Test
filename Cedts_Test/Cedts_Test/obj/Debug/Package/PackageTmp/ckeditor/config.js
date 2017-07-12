/*
Copyright (c) 2003-2011, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function (config) {
    config.language = 'zh-cn';
    config.uiColor = '#FFF';
    config.width = '100%';
    config.height = '200px';
    config.skin = 'v2';
    config.toolbar = 'Full';
    config.fullPage = false;                              //允许html,head,link等标签   
    config.removePlugins = 'elementspath';   //去掉底部显示的标签   
    config.resize_enabled = false;
    config.toolbar = [
    ['NewPage', 'Bold', 'Italic', 'Underline', 'Strike', 'NumberedList', 'BulletedList', 'Outdent', 'Indent', 'JustifyLeft',

'JustifyCenter', 'JustifyRight', 'Link', 'Unlink', 'TextColor', 'BGColor', 'Table', 'Find','ImageButton',

'RemoveFormat'], ['Styles', 'Format', 'Font', 'FontSize']
];


};
