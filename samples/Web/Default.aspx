<%@ Page Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
  <title>Silverlight.FX Samples</title>
  <style type="text/css">
  body { font-family: Segoe UI, Lucida Sans Unicode, Helvetica; font-size: 10pt; padding: 20px; }
  h1 { font-size: 18pt; font-weight: normal; color: navy; }
  h2 { font-size: 14pt; font-weight: bold; }
  hr { height: 1px; clear: both; }
  #logoImage { float: right; margin-right: 10px; margin-bottom: 10px; }
  </style>
</head>
<body>
  <img id="logoImage" src="http://projects.nikhilk.net/Content/Projects/SilverlightFX/Logo.png" />
  <h1>Silverlight.FX Samples</h1>
  <p>
    This page provides lists all the samples distributed along with
    <a href="http://projects.nikhilk.net/SilverlightFX">Silverlight.FX</a>
  </p>
  <p>Note that some samples access various web services, and require API keys. More on that below.</p>
  <hr />
  <h2>Applications</h2>
  <dl>
    <dt><a href="Amazon/AmazonSearch.aspx">Amazon Search</a></dt>
    <dd>A simple Amazon search form. One of the early viewmodel samples.</dd>
    <dt><a href="Amazon/AmazonStore.aspx">Amazon Store</a></dt>
    <dd>A bigger Amazon sample. This looks up top deals, along with providing search and the ability to
    populate your shopping cart, and completing the checkout process. The user interface demonstrates
    layout animations, transitions etc.</dd>
    <dt><a href="Flickr/FlickrTiles.aspx">Flickr Tiles</a></dt>
    <dd>A Flickr search sample with thumbnails in an animated tile layout, and images shown with
    exploding transitions.</dd>
    <dt><a href="TaskList/TaskList.aspx">TaskList</a></dt>
    <dd>A basic tasklist demonstrating view model and dialogs.</dd>
    <dt><a href="News/NewsWidget.aspx">New York Times News Widget</a></dt>
    <dd>A little widget using New York Times APIs to search and retrieve latest news articles.
    Demonstrates view model along with navigation/MVC, as well as IoC.</dd>
    <dt><a href="TwitFaves/TwitFaves.aspx">TwitFaves</a></dt>
    <dd>A twitter app that pulls out hyperlinks from your Twitter favorites. Demonstrates using
    ObjectDataSource along with your view model.</dd>
    <dt><a href="Weather/WeatherSample.aspx">Weather Widget</a></dt>
    <dd>A basic weather widget displaying current weather and forecast using zip code and
    weather.com APIs. The sample demonstrates a number of declarative effects and transitions.</dd>
  </dl>
  <hr />
  <h2>Miscellaneous/Feature-Specific Samples</h2>
  <dl>
    <dt><a href="Effects/EffectsSample.aspx">Effects and Transitions Gallery</a></dt>
    <dd>Demonstrates various effects (fade, highlight, move, rotate, etc.) and transitions
    (blinds, slide, cross fade etc.)</dd>
    <dt><a href="Effects/EffectControl.aspx">Effects in a Custom Control</a></dt>
    <dd>This sample demonstrates writing a control with an effect property, and an implemenation of a custom
    effect similar to the FlashBulb effect in powerpoint.</dd>
    <dt><a href="Themes/ThemeSample.aspx">Themes</a></dt>
    <dd>A simple demonstration of themes.</dd>
    <dt><a href="Experiments/TemplatePanelSample.aspx">TemplatePanel</a></dt>
    <dd>Demo of TemplatePanel which allows you to create a layout with named placeholders.</dd>
  </dl>
  <hr />
  <h2>API Keys</h2>
  <p>Various samples use various web services, that require access keys. You will need to update
  web.config with your own API keys. The following are the services that are in use:</p>
  <ul>
    <li>Amazon Web Services (<a href="http://aws.amazon.com/">http://aws.amazon.com/</a>)</li>
    <li>Flickr (<a href="http://www.flickr.com/services/api/">http://www.flickr.com/services/api/</a>)</li>
    <li>New York Times (<a href="http://developer.nytimes.com/docs/article_search_api">http://developer.nytimes.com/docs/article_search_api</a>)</li>
  </ul>
</body>
</html>
