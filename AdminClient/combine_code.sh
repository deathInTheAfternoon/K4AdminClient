#!/usr/bin/env bash

OUTPUT_FILE="combined_code.txt"
> "$OUTPUT_FILE"  # Overwrite existing

# Find .cs and .xaml files, skipping bin/ obj/ Images/ Assets/.
find . \
  -path "./bin" -prune -o \
  -path "./obj" -prune -o \
  -path "./Images" -prune -o \
  -path "./Assets" -prune -o \
  -type f \( -name "*.cs" -o -name "*.xaml" \) -print | while read -r file
do
  echo "====== BEGIN FILE: $file ======" >> "$OUTPUT_FILE"
  cat "$file" >> "$OUTPUT_FILE"
  echo -e "\n====== END FILE: $file ======\n" >> "$OUTPUT_FILE"
done

echo "Finished creating $OUTPUT_FILE"
