#!/bin/bash -x

find . -type f -name "*.cs" | while read -r x; do aider --sonnet --dark-mode --no-cache-prompts --no-auto-commits --no-suggest-shell-commands --yes --yes-always --message "Create unit test for file" $x; done

