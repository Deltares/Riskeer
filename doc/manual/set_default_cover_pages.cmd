@echo off
if  not exist "cover\cover-pages.pdf" (
    copy cover\default-cover-pages.pdf cover\cover-pages.pdf
    copy cover\default-chapter-ribbon.jpg cover\chapter-ribbon.jpg
)