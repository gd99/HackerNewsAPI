

# C# .NET Core Web API to access HackerNews top stories 

### Intro 
This project launches a web API to retrieve the top N stories from Hackernews. It can be run locally using visual studio, or the dockerfile instructions can be followed to run from docker. 

### How it works
It will run an OpenAI page, where the user can use the best stories V0 api, the user can select the number of stories to be returned. 

Interally it will call the best stories API, retrieving the IDs, which will then be cut down to the users specified amount. 

Each id will then be called individually, to retrieve the full data for each story. 
This is done asynchronously in parallel to speed up the operation, but there is also a config limit on the amount of open sockets the http client will open, to protect hackernews API from overload. 

A dictionary is used to keep the order of the stories, as the return of the hackernews API can't guarantee the order is kept. 


### An additional function
While testing the API's, I noticed the returned value of "descendants" didn't match the exact number of comments when you exhaust the recursive list of "kids" from a given story, which leads me to believe this value gets updated on a delay, and isn't live.
Therefore I added in the query parameter "liveCommentCount" which get the current exhaustive count of comments. This is of course much slower, as many recursive API calls are made. 



### Further changes to make 
- I set the maximum http pool size to 10, but this could be higher. Some usage testing would give the optimal rate to set it. 
- Authorisation can be added 
- Unit tests are quite basic and more could be added
- Helm or terraform could be used to improve deployment into cloud 
- Logging has been kept basic and structured logger like serilog would be an improvement 
- understand end user so any exceptions can have better messages 
- implement health status
- implement usage stats for an elk stack style monitoring 
- requirements state serving large numbers of requests, so possibly running multiple services with a load balancer would be required, assuming hackernews can handle it
- variable fields returned via API might warrant nullable/optional fields for robustness
- setup project within ci/cd environment such as gitlab or jenkins 
- create dev uat and prod env configs 