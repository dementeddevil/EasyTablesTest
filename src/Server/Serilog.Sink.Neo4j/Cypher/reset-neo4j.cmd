neo4j-shell -c 'match(n) detach delete(n);'
neo4j-shell -file cypher/time-tree.cql
neo4j-shell -file cypher/time-tree-index.cql