#!/bin/bash
path="./tests"
readarray -t tests < <(ls "$path")
for test in "${tests[@]}"
do
	echo "Testing $test"
	readarray -t files < <(ls "$path/$test" | grep ".*\.in" -o | sed -E -e "s/(.*)(\.in)/\1/")
	for file in "${files[@]}"
	do
		in="$path/$test/$file.in"
		out="$path/$test/$file.out"
		expected="$path/$test/$file.expected"
		sh program.sh "-$test" "$in" > "$out"
		DIFF=$(diff "$out" "$expected")
		if [ "$DIFF" == "" ]
		then
			echo "$file: out==expected OK"
		else
			echo "$file: out!=expected FAILED"
		fi
	done
done
exit

