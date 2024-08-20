# Architecture decision record (ADR)

An architecture decision record (ADR) is a document that captures an important architecture decision made along with its context and consequences.
This Repository contains the dotnet tool to manage the Log using templates for ADR.

## What is an architecture decision record?

In the world of technology, specifically in architecture, decisions are an everyday activity. These decisions happen in meetings, around coffee machines, and sometimes even in offsite locations. We discuss the context of the decisions with groups of people, but we often neglect to document both the decisions and the reasoning behind them. Few years later, and we find ourselves looking at the architecture, wondering, “Why did we design it this way? What were they thinking?”

To address this challenge of missing context, the solution is to document our decisions. Architecture for agile projects has to be described and defined differently. Not all decisions will be made at once, nor will all of them be done when the project begins.

Agile methods are not opposed to documentation, only to valueless documentation. Documents that assist the team itself can have value, but only if they are kept up to date. Large documents are never kept up to date. Small, modular documents have at least a chance at being updated.
Here is where ADR comes in help of this.

**An Architecture Decision Record (ADR) is a point-in-time document that records architectural decisions and the reasoning behind them.** Think of it as a snapshot that says, “On this date, given the context, drivers, and information we had, we made a decision to go with Option Z. We considered Options X, Y, and Z.”

Architecture Decision Records (ADRs) are commonly used in software design, but their benefits extend to various architectural domains such as Business, Data, Technology, and Application. We should give equal importance to “documenting decisions” as we do to creating diagrams and models, as decisions shape the context of both current and future designs. It’s essential to understand the purpose and scope of ADRs:

* **ADR serves as a communication tool**, enabling teams and stakeholders to grasp the reasoning behind decisions. It acts as a decision tracking tool, helping us trace why decisions were made, especially when initial drivers evolve.

* **ADRs are concise, usually one or two pages long.** They should be readable within an average of 5 minutes, catering to both technical and non-technical audiences.

* **ADRs are immutable**; if revisiting a decision, update the existing ADR’s status to “Superseded by XXX” and create a new ADR.

Just to summarize using specific terms:

* An **architecture decision record** (ADR) is a document that captures an important architectural decision made along with its context and consequences.
* An **architecture decision** (AD) is a software design choice that addresses a significant requirement.
* An **architecture decision log** (ADL) is the collection of all ADRs created and maintained for a particular project (or organization).
* An **architecturally-significant requirement** (ASR) is a requirement that has a measurable effect on a software system’s architecture.

Abbreviations:

* **AD**: architecture decision
* **ADL**: architecture decision log
* **ADR**: architecture decision record
* **AKM**: architecture knowledge management
* **ASR**: architecturally-significant requirement

## How to define an ADR

there are many ways to define an ADR; from the simplest to the most detailed ones. I think this repo should cover many examples: Architecture Decision Record. However, I present here my own definiton for ADR. You find below the main characteristic of ADR and how I implement it.
The whole document should be one or two pages long. We will write each ADR as if it is a conversation with a future developer. This requires good writing style, with full sentences organized into paragraphs, as described below.
Note: This is the architecture decision description that I started to use in my projects.
It is based on architecture decision description template published in ["Architecture Decisions: Demystifying Architecture" by Jeff Tyree and Art Akerman, Capital One Financial](https://www.utdallas.edu/~chung/SA/zz-Impreso-architecture_decisions-tyree-05.pdf).

* **Title**: These documents have names that are short noun phrases. For example, "ADR 1: Deployment on Ruby on Rails 3.0.10" or "ADR 9: LDAP for Multitenant Integration"
* **Status**: A decision may be "proposed" if the project stakeholders haven't agreed with it yet, or "accepted" once it is agreed. If a later ADR changes or reverses a decision, it may be marked as "deprecated" or "superseded" with a reference to its replacement.
* **Group**: You can use a simple grouping—such as integration, presentation, data, and so on—to help organize the set of decisions.
* **Context**: This section describes the forces at play, including technological, political, social, and project local. These forces are probably in tension, and should be called out as such. The language in this section is value-neutral. It is simply describing facts.
* **Assumptions**: Clearly describe the underlying assumptions in the environment in which you’re making the decision—cost, schedule, technology, and so on. Note that environmental constraints (such as accepted technology standards, enterprise architecture, commonly employed patterns, and so on) might limit the alternatives you consider.
* **Decision**: This section describes our response to these forces. It is stated in full sentences, with active voice. "We will …"
* **Decision Drivers**: Outline why you selected a position, including items such as implementation cost, total ownership cost, time to market, and required development resources’ availability. This is probably as important as the decision itself
* **Consequences**: This section describes the resulting context, after applying the decision. All consequences should be listed here, not just the "positive" ones. A particular decision may have positive, negative, and neutral consequences, but all of them affect the team and project in the future.
* **Links**: a link to other ADR if needed. Mandatory in case the status is Superseded,
* **Notes**: Because the decision-making process can take weeks, we’ve found it useful to capture notes and issues that the team discusses during the socialization process

## Repository Content

this repository contains different types of artifacts:

* **NADR.Cli**: CLI to manage ADL. the source code is available at _/src/_.
* **ADR Template**: The templates used by NADR.Cli to generate a new record. the source code is available at _/templates/adr/_.
* **Example of ADL**: some examples of ADR generate by NADR.CLi. see _/docs/adr_

### NADR.CLI

this is the tool for command line developer to simplify management of the records. in particular, you can:

* Create a new record
