"use strict";

var restify = require('restify');
var builder = require('botbuilder');
var server = restify.createServer();

server.listen(process.env.port || process.env.PORT || 3978, function () {
  console.log('%s listening to %s', server.name, server.url); 
});

var connector = new builder.ChatConnector({
  appId: process.env.MICROSOFT_APP_ID,
  appPassword: process.env.MICROSOFT_APP_PASSWORD
});

var bot = new builder.UniversalBot(connector);
bot = require("./botadapter").patch(bot);

bot.dialog('/survey', [
  function (session, args, next) {
    var prompt = ('Hello, I\'m the survey dialog. I\'m interrupting your conversation to ask you a question. Type "done" to resume');
    builder.Prompts.choice(session, prompt, "done");
  },
  function (session, results) {
    session.send("Great, back to the original conversation");
  }
]);

function startProactiveDialog(addr) {
  bot.beginDialog(savedAddress, "*:/survey", {}, { resume: true });  
}

var savedAddress;
server.post('/api/messages', connector.listen());
server.get('/api/CustomWebApi', () => {
    startProactiveDialog(savedAddress);
  }
);

bot.dialog('/', function(session, args) {

  savedAddress = session.message.address;

  var message = 'Hello! In a few seconds I\'ll send you a message proactively to demonstrate how bots can initiate messages.';
  session.send(message);
  
  connector.url
  message = 'You can also make me send a message by accessing: ';
  message += 'http://localhost:' + server.address().port + '/api/CustomWebApi';
  session.send(message);

  setTimeout(() => {
    startProactiveDialog(savedAddress);
  }, 5000)
});
