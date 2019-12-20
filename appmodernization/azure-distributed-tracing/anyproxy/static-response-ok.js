module.exports = {
    summary: 'a rule to hack response',
   
    *beforeSendRequest(requestDetail) {
        return {
            response: {
              statusCode: 200,
              header: { 'content-type': 'text/html' },
              body: 'this could be a <string> or <buffer>'
            }
          };
    }
  };

