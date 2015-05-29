@live
Feature: Initialization handlers

# See to what initialization handler each tag corresponds in 'TagMapping.txt'

Scenario: Initialization handler invocation
	Given test with defined initialization handler
	When test is about to be executed
	Then initialization handler for the test is created

@sampleTag @meaninglessTag
Scenario: Disposable initialization handler invocation
	Given test with defined initialization handler that implements IDisposable
	When test was executed
	Then initialization handler instance is disposed

@lowPriorityTag @sampleTag
Scenario: Initialization handler prioritization
	Given test with multiple defined initialization handlers
	When test is about to be executed
	Then initialization handlers are created in order of their priority