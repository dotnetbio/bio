import Util
Util.add_biodotnet_reference("Bio")
Util.add_biodotnet_reference("Bio.WebServiceHandlers")
from Bio.Web import *
from Bio.Web.Blast import *
from System.IO import *

_service = NCBIBlastHandler()

def submit_blast_search(seq, useProxy):
    "Submits a BLAST search for the given sequence.\n\
    Returns a job ID to use in polling the service."

    _search_params = BlastParameters()
    _search_params.Add("Program", "blastn")
    _search_params.Add("Database", "nr")
    _search_params.Add("Expect", "1e-10")
    _search_params.Add("CompositionBasedStatistics", "0")

    _config_params = ConfigParameters()
    _config_params.UseBrowserProxy = useProxy 
    _service.Configuration = _config_params

    job_id = _service.SubmitRequest(seq, _search_params)
    status = _service.GetRequestStatus(job_id)
    if status.Status != ServiceRequestStatus.Waiting & status.Status != ServiceRequestStatus.Ready:
        raise Exception, "Unexpected BLAST service request status: " + status.Status.ToString()
    return job_id
    
def poll_blast_results(job_id):
    "Fetches the BLAST results for the given job ID.\n\
    Returns a xml string containing BlastResult."

    _search_params = BlastParameters()
    _search_params.Add("Program", "blastn")
    _search_params.Add("Database", "nr")
    _search_params.Add("Expect", "1e-10")
    _search_params.Add("CompositionBasedStatistics", "0")

    return _service.GetResult(job_id, _search_params)

def parse_blast_results(result_string):
    "Parses the given BLAST results.\n\
    Returns a list of BlastResult objects."
    reader = StringReader(result_string)
    return _service.Parser.Parse(reader)