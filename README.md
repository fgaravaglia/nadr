# Architecture decision record (ADR)

An architecture decision record (ADR) is a document that captures an important architecture decision made along with its context and consequences.
This Repository contains the dotnet tool to manage the Log using templates for ADR.

## What is an architecture decision record?

An **architecture decision record** (ADR) is a document that captures an important architectural decision made along with its context and consequences.

An **architecture decision** (AD) is a software design choice that addresses a significant requirement.

An **architecture decision log** (ADL) is the collection of all ADRs created and maintained for a particular project (or organization).

An **architecturally-significant requirement** (ASR) is a requirement that has a measurable effect on a software system’s architecture.

Abbreviations:

* **AD**: architecture decision

* **ADL**: architecture decision log

* **ADR**: architecture decision record

* **AKM**: architecture knowledge management

* **ASR**: architecturally-significant requirement

## Repository Content

this repository contains different types of artifacts:

* **NADR.Cli**: CLI to manage ADL. the source code is available at _/src/_.
* **ADR Template**: The tempalte used by NADR.Cli to generate a new record. the source code is available at _/templates/adr/_.
* **Example of ADL**: some exambel of ADR generate by NADR.CLi. see _/docs/adr_

## NADR.CLI

this is the tool for command line developer to simplify management of the records. in particular, you can:

* Create a new record
